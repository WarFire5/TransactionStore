using AutoMapper;
using Backend.Core.Validators;
using FluentAssertions;
using FluentValidation;
using Moq;
using TransactionStore.Business.Services;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models;
using TransactionStore.Core.Models.Requests;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.Business.Tests;

public class TransactionsServiceTests
{
    private readonly Mock<ITransactionsRepository> _repositoryMock;
    private readonly Mock<ICurrencyRatesProvider> _currencyRatesProviderMock;

    private readonly IValidator<DepositWithdrawRequest> _depositWithdrawValidator;
    private readonly IValidator<TransferRequest> _transferValidator;
    private readonly IMapper _mapper;

    private readonly TransactionsService _service;

    public TransactionsServiceTests()
    {
        _repositoryMock = new Mock<ITransactionsRepository>();
        _currencyRatesProviderMock = new Mock<ICurrencyRatesProvider>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<TransactionsMappingProfile>());
        _mapper = config.CreateMapper();

        _depositWithdrawValidator = new AddDepositWithdrawValidator();
        _transferValidator = new AddTransferValidator();

        _service = new TransactionsService(
            _repositoryMock.Object,
            _mapper,
            _depositWithdrawValidator,
            _transferValidator
        );
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_Deposit_ValidTransaction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = TransactionsServiceTestData.GetValidDepositRequest(accountId);
        var transactionDto = _mapper.Map<TransactionDto>(request);

        // Act
        var result = await _service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);

        // Assert
        result.Should().Be(transactionDto.Id);
        _repositoryMock.Verify(r => r.AddDepositWithdrawTransactionAsync(It.Is<TransactionDto>(t => t.TransactionType == TransactionType.Deposit && t.Amount == 100)), Times.Once);
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_Withdraw_ValidTransaction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = TransactionsServiceTestData.GetValidWithdrawRequest(accountId);
        var transactionDto = new TransactionDto
        {
            AccountId = request.AccountId,
            TransactionType = TransactionType.Withdraw,
            CurrencyType = request.CurrencyType,
            Amount = request.Amount
        };

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<TransactionsMappingProfile>()));
        var depositWithdrawValidator = new AddDepositWithdrawValidator();
        var expectedTransactionId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.AddDepositWithdrawTransactionAsync(It.IsAny<TransactionDto>())).ReturnsAsync(expectedTransactionId);

        // Act
        var service = new TransactionsService(
            _repositoryMock.Object,
            mapper,
            depositWithdrawValidator,
            _transferValidator
        );

        var result = await service.AddDepositWithdrawTransactionAsync(TransactionType.Withdraw, request);

        // Assert
        result.Should().Be(expectedTransactionId, because: "the transaction should have been created successfully.");
        _repositoryMock.Verify(r => r.AddDepositWithdrawTransactionAsync(It.Is<TransactionDto>(t =>
            t.TransactionType == TransactionType.Withdraw && t.Amount == -100)), Times.Once);
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_ValidRequest_ReturnsTransactionId()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetDepositWithdrawRequest();
        var transactionDto = new TransactionDto
        {
            AccountId = request.AccountId,
            CurrencyType = request.CurrencyType,
            Amount = request.Amount,
            TransactionType = TransactionType.Deposit
        };

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<TransactionsMappingProfile>()));
        var depositWithdrawValidator = new AddDepositWithdrawValidator();
        var transactionId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.AddDepositWithdrawTransactionAsync(It.IsAny<TransactionDto>())).ReturnsAsync(transactionId);

        // Act
        var service = new TransactionsService(
            _repositoryMock.Object,
            mapper,
            depositWithdrawValidator,
            _transferValidator
        );

        var result = await service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);

        // Assert
        result.Should().Be(transactionId);
        _repositoryMock.Verify(r => r.AddDepositWithdrawTransactionAsync(It.Is<TransactionDto>(t =>
            t.AccountId == request.AccountId &&
            t.CurrencyType == request.CurrencyType &&
            t.Amount == request.Amount &&
            t.TransactionType == TransactionType.Deposit
        )), Times.Once);
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetDepositWithdrawRequest();
        request.Amount *= -1;
        var depositWithdrawValidator = new AddDepositWithdrawValidator();
        var validationResult = await depositWithdrawValidator.ValidateAsync(request);

        // Act
        Func<Task> act = async () => await _service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("The amount should not be less than 1. / Сумма не должна быть меньше 1.");
    }

    [Fact]
    public async Task AddTransferTransaction_ValidTransaction()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetValidTransferRequest();
        var withdrawTransactionDto = new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyFromType,
            Amount = request.Amount * -1
        };

        var rateToUSD = 1m;
        var rateFromUSD = 1 / 1.09m;
        var amountUsd = request.Amount * rateToUSD;
        var depositAmount = amountUsd * rateFromUSD;
        var depositTransactionDto = new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyToType,
            Amount = depositAmount
        };

        var transferValidator = new AddTransferValidator();
        _transferValidator.Validate(request);

        _currencyRatesProviderMock.Setup(p => p.ConvertFirstCurrencyToUsd(request.CurrencyFromType)).Returns(rateToUSD);
        _currencyRatesProviderMock.Setup(p => p.ConvertUsdToSecondCurrency(request.CurrencyToType)).Returns(rateFromUSD);

        // Act
        await _service.AddTransferTransactionAsync(request);

        // Assert
        _repositoryMock.Verify(r => r.AddTransferTransactionAsync(It.Is<TransactionDto>(t => t.AccountId == request.AccountFromId && t.Amount == -100), It.Is<TransactionDto>(t => t.AccountId == request.AccountToId && t.Amount == depositAmount)), Times.Once);
    }

    [Fact]
    public async Task AddTransferTransaction_ValidRequest_TransactionsAdded()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetTransferRequest();
        var transferValidator = new AddTransferValidator();

        // Act
        await _service.AddTransferTransactionAsync(request);

        // Assert
        _repositoryMock.Verify(r => r.AddTransferTransactionAsync(It.IsAny<TransactionDto>(), It.IsAny<TransactionDto>()), Times.Once);
    }

    [Fact]
    public void CreateWithdrawTransaction_ValidRequest_CorrectWithdrawTransaction()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetValidWithdrawRequest();
        var transactionDto = new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyFromType,
            Amount = request.Amount * -1
        };

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<TransactionsMappingProfile>()));
        var service = new TransactionsService(
            _repositoryMock.Object,
            mapper,
            _depositWithdrawValidator,
            _transferValidator
        );

        // Act
        var result = service.CreateWithdrawTransaction(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(request.AccountFromId);
        result.TransactionType.Should().Be(TransactionType.Transfer);
        result.CurrencyType.Should().Be(request.CurrencyFromType);
        result.Amount.Should().Be(-100);
    }

    [Fact]
    public void CreateDepositTransaction_ValidRequest_CorrectDepositTransaction()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetValidTransferRequest();
        var rateToUSD = 1m;
        var rateFromUSD = 1 / 1.09m;
        var amountUsd = request.Amount * rateToUSD;
        var expectedAmount = amountUsd * rateFromUSD;
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<TransactionsMappingProfile>()));
        var service = new TransactionsService(
            _repositoryMock.Object,
            mapper,
            _depositWithdrawValidator,
            _transferValidator
        );

        _currencyRatesProviderMock.Setup(p => p.ConvertFirstCurrencyToUsd(request.CurrencyFromType)).Returns(rateToUSD);
        _currencyRatesProviderMock.Setup(p => p.ConvertUsdToSecondCurrency(request.CurrencyToType)).Returns(rateFromUSD);

        var transactionDto = new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyToType,
            Amount = expectedAmount
        };

        // Act
        var result = service.CreateDepositTransaction(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(request.AccountToId);
        result.TransactionType.Should().Be(TransactionType.Transfer);
        result.CurrencyType.Should().Be(request.CurrencyToType);
        result.Amount.Should().BeApproximately(expectedAmount, 0.0001m);
    }

    [Fact]
    public async Task AddTransferTransaction_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetTransferRequest();
        request.Amount *= -1;
        var transferValidator = new AddTransferValidator();
        var validationFailures = transferValidator.Validate(request);

        // Act
        Func<Task> act = async () => await _service.AddTransferTransactionAsync(request);

        // Assert
        validationFailures.IsValid.Should().BeFalse(); // Ensure validation fails
        await act.Should().ThrowAsync<ValidationException>().WithMessage("The amount should not be less than 1. / Сумма не должна быть меньше 1.");
    }
}
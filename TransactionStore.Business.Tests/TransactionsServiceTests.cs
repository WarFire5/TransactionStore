using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TransactionStore.Business.Services;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.Business.Tests;

public class TransactionsServiceTests
{
    private readonly Mock<ITransactionsRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICurrencyRatesProvider> _currencyRatesProviderMock;
    private readonly Mock<IValidator<DepositWithdrawRequest>> _depositWithdrawValidatorMock;
    private readonly Mock<IValidator<TransferRequest>> _transferValidatorMock;

    private readonly TransactionsService _service;

    public TransactionsServiceTests()
    {
        _repositoryMock = new Mock<ITransactionsRepository>();
        _mapperMock = new Mock<IMapper>();
        _currencyRatesProviderMock = new Mock<ICurrencyRatesProvider>();
        _depositWithdrawValidatorMock = new Mock<IValidator<DepositWithdrawRequest>>();
        _transferValidatorMock = new Mock<IValidator<TransferRequest>>();

        _service = new TransactionsService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _depositWithdrawValidatorMock.Object,
            _transferValidatorMock.Object
        );
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_Deposit_ValidTransaction()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var request = TransactionsServiceTestData.GetValidDepositRequest(accountId);
        var transactionDto = new TransactionDto
        {
            AccountId = request.AccountId,
            TransactionType = TransactionType.Deposit,
            CurrencyType = request.CurrencyType,
            Amount = request.Amount
        };
        _mapperMock.Setup(m => m.Map<TransactionDto>(request)).Returns(transactionDto);
        _depositWithdrawValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

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

        _mapperMock.Setup(m => m.Map<TransactionDto>(request)).Returns(transactionDto);
        _depositWithdrawValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

        var expectedTransactionId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.AddDepositWithdrawTransactionAsync(It.IsAny<TransactionDto>())).ReturnsAsync(expectedTransactionId);

        // Act
        var result = await _service.AddDepositWithdrawTransactionAsync(TransactionType.Withdraw, request);

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

        _depositWithdrawValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<DepositWithdrawRequest>(), default))
                                     .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<TransactionDto>(request)).Returns(transactionDto);
        var transactionId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.AddDepositWithdrawTransactionAsync(It.IsAny<TransactionDto>())).ReturnsAsync(transactionId);

        // Act
        var result = await _service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);

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
        var validationFailures = new[] { new ValidationFailure("Amount", "Amount must be greater than zero.") };
        _depositWithdrawValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<DepositWithdrawRequest>(), default))
                                     .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        Func<Task> act = async () => await _service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("Amount must be greater than zero.");
    }

    [Fact]
    public async Task CreateWithdrawTransaction_ValidRequest_CorrectWithdrawTransaction()
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
        _mapperMock.Setup(m => m.Map<TransactionDto>(request)).Returns(transactionDto);

        // Act
        var result = _service.CreateWithdrawTransaction(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(request.AccountFromId);
        result.TransactionType.Should().Be(TransactionType.Transfer);
        result.CurrencyType.Should().Be(request.CurrencyFromType);
        result.Amount.Should().Be(-100);
    }

    [Fact]
    public async Task CreateDepositTransaction_ValidRequest_CorrectDepositTransaction()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetValidTransferRequest();
        var rateToUSD = 1m; // Example rate, should match setup
        var rateFromUSD = 1 / 1.09m; // Example rate, should match setup
        var amountUsd = request.Amount * rateToUSD;
        var expectedAmount = amountUsd * rateFromUSD;

        _currencyRatesProviderMock.Setup(p => p.ConvertFirstCurrencyToUsd(request.CurrencyFromType)).Returns(rateToUSD);
        _currencyRatesProviderMock.Setup(p => p.ConvertUsdToSecondCurrency(request.CurrencyToType)).Returns(rateFromUSD);

        var transactionDto = new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyToType,
            Amount = expectedAmount
        };
        _mapperMock.Setup(m => m.Map<TransactionDto>(request)).Returns(transactionDto);

        // Act
        var result = _service.CreateDepositTransaction(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(request.AccountToId);
        result.TransactionType.Should().Be(TransactionType.Transfer);
        result.CurrencyType.Should().Be(request.CurrencyToType);
        result.Amount.Should().BeApproximately(expectedAmount, 0.0001m);
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
        _transferValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
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
        _transferValidatorMock.Setup(v => v.ValidateAsync(request, default))
                              .ReturnsAsync(new ValidationResult());

        // Act
        await _service.AddTransferTransactionAsync(request);

        // Assert
        _repositoryMock.Verify(r => r.AddTransferTransactionAsync(It.IsAny<TransactionDto>(), It.IsAny<TransactionDto>()), Times.Once);
    }

    [Fact]
    public async Task AddTransferTransaction_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetTransferRequest();
        var validationFailures = new[] { new ValidationFailure("Amount", "Amount must be greater than zero.") };
        _transferValidatorMock.Setup(v => v.ValidateAsync(request, default))
                              .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        Func<Task> act = async () => await _service.AddTransferTransactionAsync(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("Amount must be greater than zero.");
    }
}
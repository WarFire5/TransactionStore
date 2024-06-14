using AutoMapper;
using Backend.Core.Validators;
using FluentAssertions;
using FluentValidation;
using Moq;
using TransactionStore.Business.Services;
using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Exceptions;
using TransactionStore.Core.Models;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;
using TransactionStore.DataLayer.Repositories;
using ValidationException = FluentValidation.ValidationException;

namespace TransactionStore.Business.Tests;

public class TransactionsServiceTests
{
    private readonly Mock<ITransactionsRepository> _repositoryMock = new();
    private readonly Mock<ICurrencyRatesProvider> _currencyRatesProviderMock = new();
    private readonly IValidator<DepositWithdrawRequest> _depositWithdrawValidator;
    private readonly IValidator<TransferRequest> _transferValidator;
    private readonly IMapper _mapper;
    private readonly TransactionsService _service;
    private readonly IMessagesService _messagesService;

    public TransactionsServiceTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<TransactionsMappingProfile>());
        _mapper = config.CreateMapper();

        _depositWithdrawValidator = new AddDepositWithdrawValidator();
        _transferValidator = new AddTransferValidator();

        _service = new TransactionsService(
            _repositoryMock.Object,
            _messagesService,
            _mapper,
            _depositWithdrawValidator,
            _transferValidator
        );
    }

    private static Guid GenerateUniqueId() => Guid.NewGuid();

    [Fact]
    public async Task AddTransaction_ValidRequest_ReturnsCorrectTransactionId()
    {
        // Arrange
        var accountId = GenerateUniqueId();
        var depositRequest = TransactionsServiceTestData.GetValidDepositRequest(accountId);
        var withdrawRequest = TransactionsServiceTestData.GetValidWithdrawRequest(accountId);
        var depositId = GenerateUniqueId();
        var withdrawId = GenerateUniqueId();

        _repositoryMock.SetupSequence(r => r.AddDepositWithdrawTransactionAsync(It.IsAny<TransactionDto>()))
            .ReturnsAsync(depositId)
            .ReturnsAsync(withdrawId);

        // Act
        var depositResult = await _service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, depositRequest);
        var withdrawResult = await _service.AddDepositWithdrawTransactionAsync(TransactionType.Withdraw, withdrawRequest);

        // Assert
        depositResult.Should().Be(depositId);
        withdrawResult.Should().Be(withdrawId);

        var depositCommission = depositRequest.Amount * 0.05m;
        var expectedDepositAmount = depositRequest.Amount - depositCommission;

        var withdrawCommission = withdrawRequest.Amount * 0.10m;
        var expectedWithdrawAmount = -(withdrawRequest.Amount + withdrawCommission);

        _repositoryMock.Verify(r => r.AddDepositWithdrawTransactionAsync(It.Is<TransactionDto>(t =>
            t.TransactionType == TransactionType.Deposit && t.Amount == expectedDepositAmount)), Times.Once);

        _repositoryMock.Verify(r => r.AddDepositWithdrawTransactionAsync(It.Is<TransactionDto>(t =>
            t.TransactionType == TransactionType.Withdraw && t.Amount == expectedWithdrawAmount)), Times.Once);
    }

    [Fact]
    public async Task AddTransaction_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidRequest = TransactionsServiceTestData.GetInvalidDepositWithdrawRequest();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, invalidRequest));
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.AddDepositWithdrawTransactionAsync(TransactionType.Withdraw, invalidRequest));
    }

    [Fact]
    public async Task AddTransferTransaction_ValidRequest_TransactionsAdded()
    {
        // Arrange
        var request = TransactionsServiceTestData.GetValidTransferRequest();

        // Act
        await _service.AddTransferTransactionAsync(request);

        // Assert
        _repositoryMock.Verify(r => r.AddTransferTransactionAsync(It.IsAny<TransactionDto>(), It.IsAny<TransactionDto>()), Times.Once);
    }

    [Fact]
    public async Task AddTransferTransaction_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidTransferRequest = TransactionsServiceTestData.GetInvalidTransferRequest();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.AddTransferTransactionAsync(invalidTransferRequest));
    }

    [Fact]
    public void CreateTransaction_ValidRequest_TransactionCreated()
    {
        // Arrange
        var transferRequest = TransactionsServiceTestData.GetValidTransferRequest();
        var commissionPercent = new CommissionsProvider().GetPercentForTransaction(TransactionType.Transfer);
        var commissionAmount = transferRequest.Amount * commissionPercent / 100;

        var expectedWithdrawTransaction = TransactionsServiceTestData.CreateExpectedWithdrawTransaction(transferRequest, commissionAmount);
        var expectedDepositTransaction = TransactionsServiceTestData.CreateExpectedDepositTransaction(transferRequest);

        // Act
        var withdrawTransaction = TransactionsService.CreateWithdrawTransaction(transferRequest, commissionAmount);
        var depositTransaction = TransactionsService.CreateDepositTransaction(transferRequest);

        // Assert
        withdrawTransaction.Should().BeEquivalentTo(expectedWithdrawTransaction);
        depositTransaction.Should().BeEquivalentTo(expectedDepositTransaction);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ValidId_ReturnsTransactions()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transactions = TransactionsServiceTestData.GetTransactions(transactionId);

        _repositoryMock.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetTransactionByIdAsync(transactionId);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<List<TransactionWithAccountIdResponse>>(transactions));
        _repositoryMock.Verify(r => r.GetTransactionByIdAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_InvalidId_ReturnsEmptyList()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var transactions = TransactionsServiceTestData.GetEmptyTransactions();

        _repositoryMock.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetTransactionByIdAsync(transactionId);

        // Assert
        result.Should().BeEmpty();
        _repositoryMock.Verify(r => r.GetTransactionByIdAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_RepositoryThrowsServiceUnavailableException_ThrowsServiceUnavailableException()
    {
        // Arrange
        var transactionId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetTransactionByIdAsync(transactionId)).ThrowsAsync(new ServiceUnavailableException("There is no connection to the database. / Нет соединения с базой данных."));

        // Act
        Func<Task> act = async () => await _service.GetTransactionByIdAsync(transactionId);

        // Assert
        await act.Should().ThrowAsync<ServiceUnavailableException>();
        _repositoryMock.Verify(r => r.GetTransactionByIdAsync(transactionId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ValidId_ReturnsTransactions()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactions = TransactionsServiceTestData.GetTransactionsByAccountId(accountId);

        _repositoryMock.Setup(r => r.GetTransactionsByAccountIdAsync(accountId)).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<List<TransactionResponse>>(transactions));
        _repositoryMock.Verify(r => r.GetTransactionsByAccountIdAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_InvalidId_ReturnsEmptyList()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactions = TransactionsServiceTestData.GetEmptyTransactions();

        _repositoryMock.Setup(r => r.GetTransactionsByAccountIdAsync(accountId)).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        result.Should().BeEmpty();
        _repositoryMock.Verify(r => r.GetTransactionsByAccountIdAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task GetTransactionsByAccountIdAsync_RepositoryThrowsServiceUnavailableException_ThrowsServiceUnavailableException()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetTransactionsByAccountIdAsync(accountId)).ThrowsAsync(new ServiceUnavailableException("There is no connection to the database. / Нет соединения с базой данных."));

        // Act
        Func<Task> act = async () => await _service.GetTransactionsByAccountIdAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ServiceUnavailableException>();
        _repositoryMock.Verify(r => r.GetTransactionsByAccountIdAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task GetBalanceByAccountIdAsync_ValidId_ReturnsBalanceResponse()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactions = TransactionsServiceTestData.GetTransactionsByAccountId(accountId);
        var expectedBalance = transactions.Sum(t => t.Amount);

        _repositoryMock.Setup(r => r.GetTransactionsByAccountIdAsync(accountId)).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetBalanceByAccountIdAsync(accountId);

        // Assert
        result.Should().BeOfType<AccountBalanceResponse>();
        result.AccountId.Should().Be(accountId);
        result.Balance.Should().Be(expectedBalance);
    }

    [Fact]
    public async Task GetBalanceByAccountIdAsync_EmptyTransactions_ReturnsZeroBalanceResponse()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactions = TransactionsServiceTestData.GetEmptyTransactions();

        _repositoryMock.Setup(r => r.GetTransactionsByAccountIdAsync(accountId)).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetBalanceByAccountIdAsync(accountId);

        // Assert
        result.Should().BeOfType<AccountBalanceResponse>();
        result.AccountId.Should().Be(accountId);
        result.Balance.Should().Be(0);
    }

    [Fact]
    public async Task GetBalanceByAccountIdAsync_RepositoryThrowsException_ThrowsServiceUnavailableException()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetTransactionsByAccountIdAsync(accountId)).ThrowsAsync(new ServiceUnavailableException("There is no connection to the database. / Нет соединения с базой данных."));

        // Act
        Func<Task> act = async () => await _service.GetBalanceByAccountIdAsync(accountId);

        // Assert
        await act.Should().ThrowAsync<ServiceUnavailableException>();
    }
}
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionStore.API.Controllers;
using TransactionStore.Business.Services;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.API.Tests;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionsService> _transactionsServiceMock;

    public TransactionsControllerTests()
    {
        _transactionsServiceMock = new Mock<ITransactionsService>();
    }

    [Theory]
    [InlineData(TransactionType.Deposit)]
    [InlineData(TransactionType.Withdraw)]
    public async Task AddDepositOrWithdrawTransaction_ReturnsCreated(TransactionType transactionType)
    {
        // Arrange
        var controller = new TransactionsController(_transactionsServiceMock.Object);
        var transactionId = Guid.NewGuid();
        _transactionsServiceMock.Setup(service => service.AddDepositWithdrawTransactionAsync(transactionType, It.IsAny<DepositWithdrawRequest>())).ReturnsAsync(transactionId);

        var request = transactionType == TransactionType.Deposit ?
            TransactionsControllerTestData.GetDepositRequest() :
            TransactionsControllerTestData.GetWithdrawRequest();

        // Act
        var result = transactionType == TransactionType.Deposit ?
            await controller.AddDepositTransaction(request) :
            await controller.AddWithdrawTransaction(request);

        // Assert
        var createdResult = result.Result as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        createdResult.Location.Should().Be($"/api/transactions/{transactionId}");
        createdResult.Value.Should().Be(transactionId);
    }

    [Fact]
    public async Task AddTransferTransaction_ReturnsCreated()
    {
        // Arrange
        var controller = new TransactionsController(_transactionsServiceMock.Object);
        var request = TransactionsControllerTestData.GetTransferRequest();

        // Act
        var result = await controller.AddTransferTransaction(request);

        // Assert
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task GetTransactionById_IdSent_OkResultReceived()
    {
        // Arrange
        var id = Guid.NewGuid();
        _transactionsServiceMock.Setup(x => x.GetTransactionByIdAsync(id)).ReturnsAsync([]);
        var controller = new TransactionsController(_transactionsServiceMock.Object);

        // Act
        var actual = await controller.GetTransactionById(id);

        // Assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _transactionsServiceMock.Verify(m => m.GetTransactionByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetTransactionById_IdSent_OkResultReceieved()
    {
        //arrange
        var id = new Guid();
        _transactionsServiceMock.Setup(x => x.GetTransactionByIdAsync(id)).ReturnsAsync(new List<TransactionWithAccountIdResponse>());
        var controller = new TransactionsController(_transactionsServiceMock.Object);

        //act
        var actual = await controller.GetTransactionById(id);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _transactionsServiceMock.Verify(m => m.GetTransactionByIdAsync(id), Times.Once);
    }
}

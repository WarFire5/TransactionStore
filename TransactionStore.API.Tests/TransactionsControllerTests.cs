using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionStore.API.Controllers;
using TransactionStore.Business.Services;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;

namespace TransactionStore.API.Tests;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionsService> _transactionsServiceMock;

    public TransactionsControllerTests()
    {
        _transactionsServiceMock = new Mock<ITransactionsService>();
    }

    [Fact]
    public async void AddDepositTransaction_ReturnsOk()
    {
        // Arrange
        var controller = new TransactionsController(_transactionsServiceMock.Object);
        _transactionsServiceMock.Setup(service => service.AddDepositWithdrawTransactionAsync(TransactionType.Deposit, It.IsAny<DepositWithdrawRequest>())).ReturnsAsync(Guid.NewGuid());

        var request = TransactionsControllerTestData.GetDepositRequest();

        // Act
        var result = await controller.AddDepositTransaction(request);

        // Assert
        var okResult = Assert.IsType<ActionResult<Guid>>(result);
    }

    [Fact]
    public async void AddWithdrawTransaction_ReturnsOk()
    {
        // Arrange
        var controller = new TransactionsController(_transactionsServiceMock.Object);
        _transactionsServiceMock.Setup(service => service.AddDepositWithdrawTransactionAsync(TransactionType.Withdraw, It.IsAny<DepositWithdrawRequest>())).ReturnsAsync(Guid.NewGuid());

        var request = TransactionsControllerTestData.GetWithdrawRequest();

        // Act
        var result = await controller.AddWithdrawTransaction(request);

        // Assert
        var okResult = Assert.IsType<ActionResult<Guid>>(result);
    }

    [Fact]
    public async void AddTransferTransaction_ReturnsOk()
    {
        // Arrange
        var controller = new TransactionsController(_transactionsServiceMock.Object);
        var request = TransactionsControllerTestData.GetTransferRequest();

        // Act
        var result = await controller.AddTransferTransaction(request);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }
}
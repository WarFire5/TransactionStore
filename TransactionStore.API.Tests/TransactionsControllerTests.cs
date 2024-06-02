using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionStore.API.Controllers;
using TransactionStore.Business.Services;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.API.Tests;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionsService> _transactionsServiceMock;

    public TransactionsControllerTests()
    {
        _transactionsServiceMock = new Mock<ITransactionsService>();
    }

    [Fact]
    public void GetTransactionsByLeadId_IdSent_OkResultReceieved()
    {
        //arrange
        var id = new Guid();
        _transactionsServiceMock.Setup(x => x.GetTransactionsByLeadId(id)).Returns(new List<TransactionsByLeadIdResponse>());
        var sut = new TransactionsController(_transactionsServiceMock.Object);

        [Fact]
        public void GetBalanceByAccountId_AccountIdSent_OkResultReceived()
        {
            // Arrange
            var accountId = new Guid();
            _transactionsServiceMock.Setup(x => x.GetBalanceByAccountId(accountId)).Returns(new AccountBalanceResponse());
            var controller = new TransactionsController(_transactionsServiceMock.Object);

            // Act
            var actual = controller.GetBalanceByAccountId(accountId);

            // Assert
            actual.Result.Should().BeOfType<OkObjectResult>();
            _transactionsServiceMock.Verify(m => m.GetBalanceByAccountId(accountId), Times.Once);
        }

        [Fact]
        public void AddDepositTransaction_ReturnsOk()
        {
            // Arrange
            var controller = new TransactionsController(_transactionsServiceMock.Object);
            _transactionsServiceMock.Setup(service => service.AddDepositWithdrawTransaction(TransactionType.Deposit, It.IsAny<DepositWithdrawRequest>())).Returns(Guid.NewGuid());

            var request = TransactionsControllerTestData.GetDepositRequest();

            // Act
            var result = controller.AddDepositTransaction(request);

            // Assert
            var okResult = Assert.IsType<ActionResult<Guid>>(result);
        }

        [Fact]
        public void AddWithdrawTransaction_ReturnsOk()
        {
            // Arrange
            var controller = new TransactionsController(_transactionsServiceMock.Object);
            _transactionsServiceMock.Setup(service => service.AddDepositWithdrawTransaction(TransactionType.Withdraw, It.IsAny<DepositWithdrawRequest>())).Returns(Guid.NewGuid());

            var request = TransactionsControllerTestData.GetWithdrawRequest();

            // Act
            var result = controller.AddWithdrawTransaction(request);

            // Assert
            var okResult = Assert.IsType<ActionResult<Guid>>(result);
        }

        [Fact]
        public void AddTransferTransaction_ReturnsOk()
        {
            // Arrange
            var controller = new TransactionsController(_transactionsServiceMock.Object);
            var request = TransactionsControllerTestData.GetTransferRequest();

            // Act
            var result = controller.AddTransferTransaction(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
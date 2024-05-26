using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionStore.API.Controllers;
using TransactionStore.Business.Services;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;

namespace TransactionStore.API.Tests
{
    public class TransactionsControllerTests
    {
        private TransactionsController CreateController(Mock<ITransactionsService> mockService)
        {
            return new TransactionsController(mockService.Object);
        }

        [Fact]
        public void AddDepositTransaction_ReturnsOk()
        {
            // Arrange
            var mockService = new Mock<ITransactionsService>();
            var controller = CreateController(mockService);

            mockService.Setup(service => service.AddDepositWithdrawTransaction(It.IsAny<DepositWithdrawRequest>())).Returns(Guid.NewGuid());

            var request = new DepositWithdrawRequest
            {
                AccountId = Guid.NewGuid(),
                TransactionType = TransactionType.Deposit,
                CurrencyType = CurrencyType.USD,
                Amount = 100
            };

            // Act
            var result = controller.AddDepositTransaction(request);

            // Assert
            var okResult = Assert.IsType<ActionResult<Guid>>(result);
        }

        [Fact]
        public void AddWithdrawTransaction_ReturnsOk()
        {
            // Arrange
            var mockService = new Mock<ITransactionsService>();
            var controller = CreateController(mockService);

            mockService.Setup(service => service.AddDepositWithdrawTransaction(It.IsAny<DepositWithdrawRequest>())).Returns(Guid.NewGuid());

            var request = new DepositWithdrawRequest
            {
                AccountId = Guid.NewGuid(),
                TransactionType = TransactionType.Withdraw,
                CurrencyType = CurrencyType.USD,
                Amount = 100
            };

            // Act
            var result = controller.AddWithdrawTransaction(request);

            // Assert
            var okResult = Assert.IsType<ActionResult<Guid>>(result);
        }

        [Fact]
        public void AddTransferTransaction_ReturnsOk()
        {
            // Arrange
            var mockService = new Mock<ITransactionsService>();
            var controller = CreateController(mockService);

            var request = new TransferRequest
            {
                AccountFromId = Guid.NewGuid(),
                AccountToId = Guid.NewGuid(),
                CurrencyFromType = CurrencyType.USD,
                CurrencyToType = CurrencyType.EUR,
                Amount = 100
            };

            // Act
            var result = controller.AddTransferTransaction(request);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void AddDepositTransaction_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionsService>();
            var controller = CreateController(mockService);

            // Act
            var result = controller.AddDepositTransaction(null);

            // Assert
            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        [Fact]
        public void AddWithdrawTransaction_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionsService>();
            var controller = CreateController(mockService);

            // Act
            var result = controller.AddWithdrawTransaction(null);

            // Assert
            Assert.IsType<ActionResult<Guid>>(result);
            var actionResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        [Fact]
        public void AddTransferTransaction_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionsService>();
            var controller = CreateController(mockService);

            // Act
            var result = controller.AddTransferTransaction(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using TransactionStore.Core.DTOs;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.DataLayer.Tests
{
    public class TransactionsRepositoryTests
    {
        [Fact]
        public void AddDepositWithdrawTransaction_ValidTransaction_ReturnsId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new TransactionStoreContext(options))
            {
                var repository = new TransactionsRepository(context);
                var transaction = new TransactionDto { Id = Guid.NewGuid() };

                // Act
                var result = repository.AddDepositWithdrawTransaction(transaction);

                // Assert
                Assert.Equal(transaction.Id, result);
            }
        }

        [Fact]
        public void AddTransferTransaction_ValidTransactions_NoExceptionsThrown()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new TransactionStoreContext(options))
            {
                var repository = new TransactionsRepository(context);
                var transferWithdraw = new TransactionDto { Id = Guid.NewGuid() };
                var transferDeposit = new TransactionDto { Id = Guid.NewGuid() };

                // Act
                repository.AddTransferTransaction(transferWithdraw, transferDeposit);
            }
        }

        [Fact]
        public void AddDepositWithdrawTransaction_NullTransaction_ThrowsException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new TransactionStoreContext(options))
            {
                var repository = new TransactionsRepository(context);

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => repository.AddDepositWithdrawTransaction(null));
            }
        }

        [Fact]
        public void AddTransferTransaction_NullTransferWithdraw_ThrowsException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new TransactionStoreContext(options))
            {
                var repository = new TransactionsRepository(context);
                var transferDeposit = new TransactionDto { Id = Guid.NewGuid() };

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => repository.AddTransferTransaction(null, transferDeposit));
            }
        }

        [Fact]
        public void AddTransferTransaction_NullTransferDeposit_ThrowsException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new TransactionStoreContext(options))
            {
                var repository = new TransactionsRepository(context);
                var transferWithdraw = new TransactionDto { Id = Guid.NewGuid() };

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => repository.AddTransferTransaction(transferWithdraw, null));
            }
        }
    }
}
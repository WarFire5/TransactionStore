using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using TransactionStore.Core.DTOs;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.DataLayer.Tests
{
    public class TransactionsRepositoryTests
    {
        private readonly Mock<DbSet<TransactionDto>> _transactionsMock;
        private readonly TransactionStoreContext _transactionStoreContext;

        public TransactionsRepositoryTests()
        {
            // Создание мокированного списка транзакций
            var transactions = TestData.GetFakeTransactionDtoList();
            _transactionsMock = transactions.AsQueryable().BuildMockDbSet();

            // Настройка параметров контекста
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _transactionStoreContext = new TransactionStoreContext(options);
            _transactionStoreContext.Transactions = _transactionsMock.Object;
        }

        [Fact]
        public void GetTransactionsByAccountId_GuidSent_ListTransactionsDtoReceived()
        {
            // Arrange
            var accountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1");
            var expected = TestData.GetFakeTransactionDtoList().Where(t => t.AccountId == accountId).ToList();

            var sut = new TransactionsRepository(_transactionStoreContext);

            // Act
            var actual = sut.GetBalanceByAccountId(accountId);

            // Assert
            Assert.NotNull(actual);
            actual.Should().BeEquivalentTo(expected);
        }

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
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransactionStore.Core.DTOs;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.DataLayer.Tests;

public class TransactionsRepositoryTests
{
    private readonly Mock<DbSet<TransactionDto>> _transactionsMock;
    private readonly TransactionStoreContext _transactionStoreContext;

    public TransactionsRepositoryTests()
    {
        private readonly TransactionStoreContext _transactionStoreContext;

        public TransactionsRepositoryTests()
        {
            // ��������� ���������� ���������
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _transactionStoreContext = new TransactionStoreContext(options);
        }

        [Fact]
        public void GetTransactionsByAccountId_GuidSent_ListTransactionsDtoReceived()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TransactionStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new TransactionStoreContext(options))
            {
                var repository = new TransactionsRepository(context);
                var accountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1");

                var transaction = new TransactionDto { AccountId = accountId, Amount = 100 };
                context.Transactions.Add(transaction);
                context.SaveChanges();

                var expected = new List<TransactionDto>() { transaction };

                // Act
                var result = repository.GetBalanceByAccountId(accountId);

                // Assert
                Assert.NotNull(result);
                result.Should().BeEquivalentTo(expected);
            }
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

                //Assert
                var result = context.Transactions.ToList();
                result.Count.Should().Be(2);
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

    [Fact]
    public void GetTransactionsByLeadId_GuidSent_ListTransactionsDtoReceived()
    {
        // Arrange
        var id = new Guid("550df504-032e-4ef7-aee2-53cf66e4d0c8");
        var expected = TransactionsRepositoryTestData.GetFakeFullTransactionDtoList().Where(t => t.Id == id).ToList();

        var sut = new TransactionsRepository(_transactionStoreContext);

        // Act
        var actual = sut.GetTransactionsByLeadId(id);

        // Assert
        Assert.NotNull(actual);
        actual.Should().BeEquivalentTo(expected);
    }
}

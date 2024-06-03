using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransactionStore.Core.DTOs;
using TransactionStore.DataLayer;
using TransactionStore.DataLayer.Repositories;

public class TransactionsRepositoryTests
{
    private readonly TransactionStoreContext _transactionStoreContext;

    public TransactionsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TransactionStoreContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _transactionStoreContext = new TransactionStoreContext(options);
    }

    [Fact]
    public async Task GetTransactionsByAccountId_GuidSent_ListTransactionsDtoReceived()
    {
        // Arrange
        var accountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1");

        var options = new DbContextOptionsBuilder<TransactionStoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ”никальное им€ базы данных
            .Options;

        using (var context = new TransactionStoreContext(options))
        {
            var repository = new TransactionsRepository(context);

            var transaction = new TransactionDto { AccountId = accountId, Amount = 100 };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            var expected = new List<TransactionDto> { transaction };

            // Act
            var result = await repository.GetBalanceByAccountIdAsync(accountId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    [Fact]
    public async Task GetTransactionsByLeadId_GuidSent_ListTransactionsDtoReceived()
    {
        // Arrange
        var leadId = new Guid("550df504-032e-4ef7-aee2-53cf66e4d0c8");

        var options = new DbContextOptionsBuilder<TransactionStoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ”никальное им€ базы данных
            .Options;

        using (var context = new TransactionStoreContext(options))
        {
            var repository = new TransactionsRepository(context);

            var transaction = new TransactionDto { Id = leadId, AccountId = Guid.NewGuid(), Amount = 100 };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            var expected = new List<TransactionDto> { transaction };

            // Act
            var result = await repository.GetTransactionsByLeadIdAsync(leadId);

            // Assert
            result.Should().BeEquivalentTo(expected);

            // Cleanup
            await context.Database.EnsureDeletedAsync();
        }
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_ValidTransaction_ReturnsId()
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
            var result = await repository.AddDepositWithdrawTransactionAsync(transaction);

            // Assert
            result.Should().Be(transaction.Id);
        }
    }

    [Fact]
    public async Task AddTransferTransaction_ValidTransactions_NoExceptionsThrown()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TransactionStoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // ”никальное им€ базы данных
            .Options;

        using (var context = new TransactionStoreContext(options))
        {
            var repository = new TransactionsRepository(context);
            var transferWithdraw = new TransactionDto { Id = Guid.NewGuid() };
            var transferDeposit = new TransactionDto { Id = Guid.NewGuid() };

            // Act
            await repository.AddTransferTransactionAsync(transferWithdraw, transferDeposit);

            // Assert
            var result = context.Transactions.ToList();
            result.Count.Should().Be(2);
        }
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_NullTransaction_ThrowsException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TransactionStoreContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var context = new TransactionStoreContext(options))
        {
            var repository = new TransactionsRepository(context);

            // Act & Assert
            Func<Task> act = async () => await repository.AddDepositWithdrawTransactionAsync(null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }

    [Fact]
    public async Task AddTransferTransaction_NullTransferWithdraw_ThrowsException()
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
            Func<Task> act = async () => await repository.AddTransferTransactionAsync(null, transferDeposit);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }

    [Fact]
    public async Task AddTransferTransaction_NullTransferDeposit_ThrowsException()
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
            Func<Task> act = async () => await repository.AddTransferTransactionAsync(transferWithdraw, null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
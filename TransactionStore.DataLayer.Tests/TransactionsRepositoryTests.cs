using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransactionStore.Core.DTOs;
using TransactionStore.DataLayer;
using TransactionStore.DataLayer.Repositories;

public class TransactionsRepositoryTests
{
    private DbContextOptions<TransactionStoreContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<TransactionStoreContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetTransactionsByAccountId_GuidSent_ListTransactionsDtoReceived()
    {
        // Arrange
        var accountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1");

        var options = CreateNewContextOptions();

        using (var context = new TransactionStoreContext(options))
        {
            var repository = new TransactionsRepository(context);

            var transaction = new TransactionDto { AccountId = accountId, Amount = 100 };
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            var expected = new List<TransactionDto> { transaction };

            // Act
            var result = await repository.GetTransactionsByAccountIdAsync(accountId);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    [Fact]
    public async Task GetTransactionsByLeadId_GuidSent_ListTransactionsDtoReceived()
    {
        // Arrange
        var leadId = new Guid("550df504-032e-4ef7-aee2-53cf66e4d0c8");

        var options = CreateNewContextOptions();

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
        }
    }

    [Fact]
    public async Task AddDepositWithdrawTransaction_ValidTransaction_ReturnsId()
    {
        // Arrange
        var options = CreateNewContextOptions();

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
        var options = CreateNewContextOptions();

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
        var options = CreateNewContextOptions();

        using (var context = new TransactionStoreContext(options))
        {
            var repository = new TransactionsRepository(context);

            // Act & Assert
            Func<Task> act = async () => await repository.AddDepositWithdrawTransactionAsync(null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }

    [Fact]
    public async Task AddTransferTransaction_NullTransaction_ThrowsException()
    {
        // Arrange
        var options = CreateNewContextOptions();

        using (var context = new TransactionStoreContext(options))
        {
            var repository = new TransactionsRepository(context);

            // Act & Assert
            Func<Task> actWithdraw = async () => await repository.AddTransferTransactionAsync(null, new TransactionDto { Id = Guid.NewGuid() });
            await actWithdraw.Should().ThrowAsync<ArgumentNullException>();

            Func<Task> actDeposit = async () => await repository.AddTransferTransactionAsync(new TransactionDto { Id = Guid.NewGuid() }, null);
            await actDeposit.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
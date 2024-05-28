using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using TransactionStore.Core.DTOs;
using TransactionStore.DataLayer.Repositories;

namespace TransactionStore.DataLayer.Tests;

public class TransactionsRepositoryTests
{
    private readonly Mock<DbSet<TransactionDto>> _transactionsMock;
    private readonly TransactionStoreContext _transactionStoreContext;

    public TransactionsRepositoryTests()
    {
        var transactions = TransactionsRepositoryTestData.GetFakeTransactionDtoList();
        _transactionsMock = transactions.AsQueryable().BuildMockDbSet();

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
        var expected = TransactionsRepositoryTestData.GetFakeTransactionDtoList().Where(t => t.AccountId == accountId).ToList();

        var sut = new TransactionsRepository(_transactionStoreContext);

        // Act
        var actual = sut.GetTransactionsByAccountId(accountId);

        // Assert
        Assert.NotNull(actual);
        actual.Should().BeEquivalentTo(expected);
    }
}

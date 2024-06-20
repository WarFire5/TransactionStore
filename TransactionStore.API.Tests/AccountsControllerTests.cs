using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionStore.API.Controllers;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.API.Tests;

public class AccountsControllerTests
{
    private readonly Mock<ITransactionsService> _transactionsServiceMock;

    public AccountsControllerTests()
    {
        _transactionsServiceMock = new Mock<ITransactionsService>();
    }

    [Fact]
    public async Task GetTransactionsByAccountId_AccountIdSent_OkResultReceived()
    {
        // Arrange
        var accountId = new Guid();
        _transactionsServiceMock.Setup(x => x.GetTransactionsByAccountIdAsync(accountId)).ReturnsAsync([]);
        var sut = new AccountsController(_transactionsServiceMock.Object);

        // Act
        var result = await sut.GetTransactionsByAccountId(accountId);

        // Assert
        result.Should().BeOfType<ActionResult<List<TransactionResponse>>>();
        _transactionsServiceMock.Verify(m => m.GetTransactionsByAccountIdAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task GetBalanceByAccountId_AccountIdSent_OkResultReceived()
    {
        // Arrange
        var accountId = new Guid();
        _transactionsServiceMock.Setup(x => x.GetBalanceByAccountIdAsync(accountId)).ReturnsAsync(new AccountBalanceResponse());
        var sut = new AccountsController(_transactionsServiceMock.Object);

        // Act
        var result = await sut.GetBalanceByAccountId(accountId);

        // Assert
        result.Should().BeOfType<ActionResult<AccountBalanceResponse>>();
        _transactionsServiceMock.Verify(m => m.GetBalanceByAccountIdAsync(accountId), Times.Once);
    }
}
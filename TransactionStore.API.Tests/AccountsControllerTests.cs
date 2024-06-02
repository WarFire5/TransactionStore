using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionStore.API.Controllers;
using TransactionStore.Business.Services;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.API.Tests;

public class AccountsControllerTests
{
    private readonly Mock<ITransactionsService> _transactionsServiceMock;

    public AccountsControllerTests()
    {
        _transactionsServiceMock = new Mock<ITransactionsService>();
    }

    [Fact]
    public void GetBalanceByAccountId_AccountIdSent_OkResultReceieved()
    {
        //arrange
        var accountId = new Guid();
        _transactionsServiceMock.Setup(x => x.GetBalanceByAccountId(accountId)).Returns(new AccountBalanceResponse());
        var sut = new AccountsController(_transactionsServiceMock.Object);

        //act
        var actual = sut.GetBalanceByAccountId(accountId);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _transactionsServiceMock.Verify(m => m.GetBalanceByAccountId(accountId), Times.Once);
    }

    [Fact]
    public void GetTransactionsByAccountId_AccountIdSent_OkResultReceieved()
    {
        //arrange
        var accountId = new Guid();
        _transactionsServiceMock.Setup(x => x.GetTransactionsByAccountId(accountId)).Returns(new List<TransactionResponse>());
        var sut = new AccountsController(_transactionsServiceMock.Object);

        //act
        var actual = sut.GetTransactionsByAccountId(accountId);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _transactionsServiceMock.Verify(m => m.GetTransactionsByAccountId(accountId), Times.Once);
    }
}
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionStore.API.Controllers;
using TransactionStore.Business.Services;
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
    public void GetTransactionsByAccountId_AccountIdSent_OkResultReceieved()
    {
        //arrange
        var accountId = new Guid();
        _transactionsServiceMock.Setup(x => x.GetTransactionsByAccountId(accountId)).Returns(new List<TransactionsByAccountIdResponse>());
        var sut = new TransactionsController(_transactionsServiceMock.Object);

        //act
        var actual = sut.GetTransactionsByAccountId(accountId);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _transactionsServiceMock.Verify(m => m.GetTransactionsByAccountId(accountId), Times.Once);
    }
}
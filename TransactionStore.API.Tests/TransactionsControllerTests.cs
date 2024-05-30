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
    public void GetTransactionsByLeadId_IdSent_OkResultReceieved()
    {
        //arrange
        var id = new Guid();
        _transactionsServiceMock.Setup(x => x.GetTransactionsByLeadId(id)).Returns(new List<TransactionsByLeadIdResponse>());
        var sut = new TransactionsController(_transactionsServiceMock.Object);

        //act
        var actual = sut.GetTransactionsByLeadId(id);

        //assert
        actual.Result.Should().BeOfType<OkObjectResult>();
        _transactionsServiceMock.Verify(m => m.GetTransactionsByLeadId(id), Times.Once);
    }
}
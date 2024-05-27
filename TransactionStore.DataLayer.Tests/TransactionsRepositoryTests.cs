using Moq;
using MockQueryable.Moq;
using TransactionStore.DataLayer.Repositories;
using TransactionStore.Core.DTOs;
using FluentAssertions;

namespace TransactionStore.DataLayer.Tests
{
    public class TransactionsRepositoryTests
    {
        private readonly Mock<TransactionStoreContext> _transactionStoreContextMock;

        public TransactionsRepositoryTests()
        {
            _transactionStoreContextMock = new Mock<TransactionStoreContext>();
        }

        [Fact]
        public void GetTransactionsByAccountId_GuidSent_ListTransactionsDtoReceieved()
        {
            //arrange
            var expected = new List<TransactionDto>()
            {
            TestData.GetFakeTransactionDtoList()[0],
            TestData.GetFakeTransactionDtoList()[1],
            TestData.GetFakeTransactionDtoList()[2]
            };

            var accountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1");
            var mock = TestData.GetFakeTransactionDtoList().BuildMock().BuildMockDbSet();
            _transactionStoreContextMock.Setup(x=>x.Transactions).Returns(mock.Object);
            var sut = new TransactionsRepository(_transactionStoreContextMock.Object);

            //act
            var actual = sut.GetTransactionsByAccountId(accountId);

            //assert
            Assert.NotNull(actual);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
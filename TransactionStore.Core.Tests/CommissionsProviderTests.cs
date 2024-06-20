using FluentAssertions;
using TransactionStore.Core.Data;

namespace TransactionStore.Core.Tests;

public enum TestTransaction
{
    DEPOSIT,
    WITHDRAW,
    TRANSFER,
    UNKNOWN // для проверки исключений
}

public class CommissionsProviderTests
{
    private readonly CommissionsProvider _commissionsProvider;

    public CommissionsProviderTests()
    {
        _commissionsProvider = new CommissionsProvider();
    }

    [Theory]
    [InlineData(TestTransaction.DEPOSIT, 5)]
    [InlineData(TestTransaction.WITHDRAW, 10)]
    [InlineData(TestTransaction.TRANSFER, 7.5)]
    public void GetPercentForTransaction_ExistingTransactionType_ReturnsPercent(TestTransaction transactionType, decimal expectedPercent)
    {
        // Act
        var percent = _commissionsProvider.GetPercentForTransaction(transactionType);

        // Assert
        percent.Should().Be(expectedPercent);
    }

    [Fact]
    public void GetPercentForTransaction_NonExistingTransactionType_ThrowsArgumentException()
    {
        // Arrange
        Enum nonExistingType = TestTransaction.UNKNOWN; // Неизвестный тип транзакции

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _commissionsProvider.GetPercentForTransaction(nonExistingType));
        Assert.Contains("The commission percentage for transaction of type UNKNOWN not found. / Процент комиссии для транзакции типа UNKNOWN не найден.", exception.Message);
    }
}
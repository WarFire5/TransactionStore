using FluentAssertions;
using TransactionStore.Core.Data;
using TransactionStore.Core.Settings;

namespace TransactionStore.Core.Tests;

public enum TestTransaction
{
    DEPOSIT,
    WITHDRAW,
    TRANSFER,
    UNKNOWN // для проверки исключений
}

public class CommissionsProviderTests(ComissionSettings comission)
{
    private readonly CommissionsProvider _commissionsProvider = new(comission);

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
        Assert.Contains("The commission percentage for transaction of type UNKNOWN not found.", exception.Message);
    }
}
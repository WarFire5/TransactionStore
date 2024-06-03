using FluentAssertions;
using TransactionStore.Core.Data;

namespace TransactionStore.Core.Tests;

public enum TestCurrency
{
    USD,
    RUB,
    EUR,
    JPY,
    CNY,
    RSD,
    BGN,
    ARS,
    INVALID // для проверки исключений
}

public class CurrencyRatesProviderTests
{
    private readonly CurrencyRatesProvider _currencyRatesProvider;

    public CurrencyRatesProviderTests()
    {
        _currencyRatesProvider = new CurrencyRatesProvider();
    }

    [Theory]
    [InlineData(TestCurrency.USD, 1)]
    [InlineData(TestCurrency.RUB, 0.011)]
    [InlineData(TestCurrency.EUR, 1.09)]
    [InlineData(TestCurrency.JPY, 0.0064)]
    [InlineData(TestCurrency.CNY, 0.14)]
    [InlineData(TestCurrency.RSD, 0.0093)]
    [InlineData(TestCurrency.BGN, 0.56)]
    [InlineData(TestCurrency.ARS, 0.0011)]
    public async Task ConvertFirstCurrencyToUsd_ValidCurrency_ReturnsCorrectRate(TestCurrency currency, decimal expectedRate)
    {
        // Act
        var rate = await _currencyRatesProvider.ConvertFirstCurrencyToUsdAsync(currency);

        // Assert
        rate.Should().Be(expectedRate);
    }

    [Theory]
    [InlineData(TestCurrency.USD, 1)]
    [InlineData(TestCurrency.RUB, 90.9091)]
    [InlineData(TestCurrency.EUR, 0.917431)]
    [InlineData(TestCurrency.JPY, 156.25)]
    [InlineData(TestCurrency.CNY, 7.142857)]
    [InlineData(TestCurrency.RSD, 107.5269)]
    [InlineData(TestCurrency.BGN, 1.785714)]
    [InlineData(TestCurrency.ARS, 909.0909)]
    public async Task ConvertUsdToSecondCurrency_ValidCurrency_ReturnsCorrectRate(TestCurrency currency, decimal expectedRate)
    {
        // Act
        var rate = await _currencyRatesProvider.ConvertUsdToSecondCurrencyAsync(currency);

        // Assert
        rate.Should().BeApproximately(expectedRate, 0.0001m); // Из-за возможных ошибок округления
    }

    [Fact]
    public async Task ConvertFirstCurrencyToUsd_InvalidCurrency_ThrowsArgumentException()
    {
        // Act
        async Task Act() => await _currencyRatesProvider.ConvertFirstCurrencyToUsdAsync(TestCurrency.INVALID);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }

    [Fact]
    public async Task ConvertUsdToSecondCurrency_InvalidCurrency_ThrowsArgumentException()
    {
        // Act
        async Task Act() => await _currencyRatesProvider.ConvertUsdToSecondCurrencyAsync(TestCurrency.INVALID);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }
}
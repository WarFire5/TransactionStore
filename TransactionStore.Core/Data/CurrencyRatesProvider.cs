namespace TransactionStore.Core.Data;

public class CurrencyRatesProvider : ICurrencyRatesProvider
{
    private readonly Dictionary<string, decimal> _rates;

    public CurrencyRatesProvider()
    {
        _rates = new Dictionary<string, decimal>()
            {
                { "USD", 1m },
                { "RUB", 0.011m },
                { "EUR", 1.09m },
                { "JPY", 0.0064m },
                { "CNY", 0.14m },
                { "RSD", 0.0093m },
                { "BGN", 0.56m },
                { "ARS", 0.0011m }
            };
    }

    private static string ConvertCurrencyTypeEnumToString(Enum currencyType)
    {
        return currencyType.ToString().ToUpper();
    }

    public decimal ConvertFirstCurrencyToUsd(Enum currencyType)
    {
        var currency = ConvertCurrencyTypeEnumToString(currencyType);
        if (_rates.TryGetValue(currency, out var rateToUsd))
        {
            return rateToUsd;
        }
        throw new ArgumentException($"Rate for {currency} to USD not found. / Курс {currency} к USD не найден.");
    }

    public decimal ConvertUsdToSecondCurrency(Enum currencyType)
    {
        var currency = ConvertCurrencyTypeEnumToString(currencyType);
        if (_rates.TryGetValue(currency, out var rateToUsd))
        {
            return 1 / rateToUsd;
        }
        throw new ArgumentException($"Rate for USD to {currency} not found. / Курс USD к {currency} не найден.");
    }
}
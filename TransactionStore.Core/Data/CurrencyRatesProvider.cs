using Serilog;

namespace TransactionStore.Core.Data;

public class CurrencyRatesProvider : ICurrencyRatesProvider
{
    private readonly Dictionary<string, decimal> _rates;
    private readonly ILogger _logger = Log.ForContext<CurrencyRatesProvider>();

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

    private static string ConvertCurrencyEnumToString(Enum currencyNumber)
    {
        return currencyNumber.ToString().ToUpper();
    }

    public decimal ConvertFirstCurrencyToUsd(Enum currencyNumber)
    {
        var currency = ConvertCurrencyEnumToString(currencyNumber);
        if (_rates.TryGetValue(currency, out var rateToUsd))
        {
            _logger.Information($"Returning rate {currency} to USD - {rateToUsd}. / Возврат курса {currency} к USD – {rateToUsd}");
            return rateToUsd;
        }
        _logger.Error($"Throwing an error if rate for {currency} to USD not found. / Выдача ошибки, если курс {currency} к USD не найден.");
        throw new ArgumentException($"Rate for {currency} to USD not found. / Курс {currency} к USD не найден.");
    }

    public decimal ConvertUsdToSecondCurrency(Enum currencyNumber)
    {
        var currency = ConvertCurrencyEnumToString(currencyNumber);
        if (_rates.TryGetValue(currency, out var rateToUsd))
        {
            _logger.Information($"Returning rate USD to {currency} - 1/{rateToUsd}. /  Возврат курса USD к {currency} - 1/{rateToUsd}.");
            return 1 / rateToUsd;
        }
        _logger.Error($"Throwing an error if rate for USD to {currency} not found. / Выдача ошибки, если курс USD к {currency} не найден.");
        throw new ArgumentException($"Rate for USD to {currency} not found. / Курс USD к {currency} не найден.");
    }
}
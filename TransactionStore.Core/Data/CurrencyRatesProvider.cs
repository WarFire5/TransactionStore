using Messaging.Shared;
using Serilog;

namespace TransactionStore.Core.Data;

public class CurrencyRatesProvider : ICurrencyRatesProvider
{
    private static Dictionary<string, decimal> _rates;
    private readonly ILogger _logger = Log.ForContext<CurrencyRatesProvider>();

    public CurrencyRatesProvider()
    {

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
            _logger.Information($"Returning rate {currency} to USD - {rateToUsd}.");
            return rateToUsd;
        }

        _logger.Error($"Throwing an error if rate for {currency} to USD not found.");
        throw new ArgumentException($"Rate for {currency} to USD not found.");
    }

    public decimal ConvertUsdToSecondCurrency(Enum currencyNumber)
    {
        var currency = ConvertCurrencyEnumToString(currencyNumber);
        if (_rates.TryGetValue(currency, out var rateToUsd))
        {
            _logger.Information($"Returning rate USD to {currency} - 1/{rateToUsd}.");
            return 1 / rateToUsd;
        }

        _logger.Error($"Throwing an error if rate for USD to {currency} not found.");
        throw new ArgumentException($"Rate for USD to {currency} not found.");
    }

    public void SetRates(RatesInfo rates)
    {
        _logger.Information($"Currency rates updated at {DateTime.Now}.");
        _rates = rates.Rates;
    }
}
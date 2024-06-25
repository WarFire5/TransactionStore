using Messaging.Shared;
using Serilog;

namespace TransactionStore.Core.Data;

public class CurrencyRatesProvider : ICurrencyRatesProvider
{
    private Dictionary<string, decimal> _rates;
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
        if (_rates== null) 
        {
            _logger.Error("Error, currency rates not found.");
            throw new ArgumentException("Error, currency rates not found.");
        }
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

    public void SetRates(RatesInfo rates)
    {
        _logger.Information("Rates updated at " + DateTime.Now.ToString("dd.MM.yyyy HH.mm.ss"));
        _rates = rates.Rates;
        _logger.Information($"{_rates}");
    }
}
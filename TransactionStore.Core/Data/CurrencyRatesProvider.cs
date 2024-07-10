using Messaging.Shared;
using Serilog;

namespace TransactionStore.Core.Data;

public class CurrencyRatesProvider : ICurrencyRatesProvider
{
    private static readonly object _lockObject = new();
    private static Dictionary<string, decimal> _rates = [];
    private readonly ILogger _logger = Log.ForContext<CurrencyRatesProvider>();

    private static string ConvertCurrencyEnumToString(Enum currencyNumber)
    {
        return currencyNumber.ToString().ToUpper();
    }

    public decimal ConvertFirstCurrencyToUsd(Enum currencyNumber)
    {
        var currency = ConvertCurrencyEnumToString(currencyNumber);
        lock (_lockObject)
        {
            if (_rates.TryGetValue(currency, out var rateToUsd))
            {
                _logger.Information($"Возвращаем курс {currency} к USD - {rateToUsd}.");
                return rateToUsd;
            }
        }

        _logger.Error($"Курс для {currency} к USD не найден.");
        throw new ArgumentException($"Курс для {currency} к USD не найден.");
    }

    public decimal ConvertUsdToSecondCurrency(Enum currencyNumber)
    {
        var currency = ConvertCurrencyEnumToString(currencyNumber);
        lock (_lockObject)
        {
            if (_rates.TryGetValue(currency, out var rateToUsd))
            {
                _logger.Information($"Возвращаем курс USD к {currency} - 1/{rateToUsd}.");
                return 1 / rateToUsd;
            }
        }

        _logger.Error($"Курс для USD к {currency} не найден.");
        throw new ArgumentException($"Курс для USD к {currency} не найден.");
    }

    public void SetRates(RatesInfo rates)
    {
        lock (_lockObject)
        {
            _logger.Information($"Обновление курсов валют на {DateTime.Now}.");
            _rates = rates.Rates;
        }
    }
}
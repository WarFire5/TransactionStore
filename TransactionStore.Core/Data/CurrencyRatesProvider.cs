using Messaging.Shared;
using Serilog;
using System.Text.Json;
using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Data;

public class CurrencyRatesProvider : ICurrencyRatesProvider
{
    private Dictionary<string, decimal> _rates;
    private readonly ILogger _logger = Log.ForContext<CurrencyRatesProvider>();

    public CurrencyRatesProvider()
    {
        LoadRatesFromFile();
    }

    private void LoadRatesFromFile()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.json");

        _logger.Information($"Attempting to load currency rates from file: {filePath}");

        if (File.Exists(filePath))
        {
            try
            {
                var json = File.ReadAllText(filePath);
                _rates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);

                _logger.Information("Currency rates loaded from file successfully.");
                _logger.Information($"Loaded currency rates: {JsonSerializer.Serialize(_rates)}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error reading currency rates from file. Initializing with an empty dictionary in case of error.");
                _rates = [];
            }
        }
        else
        {
            _logger.Warning("Currency rates file not found, initializing with empty currency rates.");
            _rates = [];
        }
    }

    private static string ConvertCurrencyEnumToString(Enum currencyNumber)
    {
        return currencyNumber.ToString().ToUpper();
    }

    public decimal ConvertFirstCurrencyToUsd(Enum currencyNumber)
    {
        var currency = ConvertCurrencyEnumToString(currencyNumber);
        if (_rates == null)
        {
            _logger.Error("Error, currency rates not found.");
            throw new ArgumentException("Error, currency rates not found.");
        }

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

        _logger.Information($"Started filtering currency rates on {DateTime.Now}.");
        var trimmedRates = GetSecondCurrencyFromPair();
        var result = GetFilteredCurrencyRates(trimmedRates);
        _logger.Information($"Finished filtering currency rates on {DateTime.Now}.");

        _rates = result;
        _logger.Information($"The following currency rates were obtained: {result}.");

        _logger.Information($"Serializing the result in JSON and write it to a file.");
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.json");

        try
        {
            File.WriteAllText(filePath, json);
            _logger.Information($"Result has been written to {filePath}.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error writing result to file.");
        }
    }

    private Dictionary<string, decimal> GetSecondCurrencyFromPair()
    {
        _logger.Information("Getting the second currency from the pair.");
        var newRates = new Dictionary<string, decimal>();

        foreach (var rate in _rates)
        {
            var trimKey = rate.Key.Remove(0, 3);
            if (!newRates.ContainsKey(trimKey))
            {
                newRates.Add(trimKey, rate.Value);
            }
        }

        return newRates;
    }

    private Dictionary<string, decimal> GetFilteredCurrencyRates(Dictionary<string, decimal> oldDictionary)
    {
        var validKeys = Enum.GetNames(typeof(Currency));

        _logger.Information("Getting filtered currency rates.");
        var filteredDictionary = oldDictionary.Where(rate => validKeys.Contains(rate.Key)).ToDictionary();

        if (!filteredDictionary.ContainsKey("USD"))
        {
            filteredDictionary.Add("USD", 1);
        }

        filteredDictionary["USD"] = 1;

        return filteredDictionary;
    }
}
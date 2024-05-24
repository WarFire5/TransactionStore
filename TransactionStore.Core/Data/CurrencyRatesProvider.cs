namespace TransactionStore.Core.Data;

public class CurrencyRatesProvider
{
    private readonly Dictionary<string, decimal> _rate;

    public CurrencyRatesProvider()
    {
        _rate = new Dictionary<string, decimal>()
        {
            { "USDUSD", 1m },

            { "RUBUSD", 0.011m },
            { "EURUSD", 1.09m },
            { "JPYUSD", 0.0064m },
            { "CNYUSD", 0.14m },
            { "RSDUSD", 0.0093m },
            { "BGNUSD", 0.56m },
            { "ARSUSD", 0.0011m },

            { "USDRUB", 90.89m },
            { "USDEUR", 0.92m },
            { "USDJPY", 155.66m },
            { "USDCNY", 7.22m },
            { "USDRSD", 107.50m },
            { "USDBGN", 1.80m },
            { "USDARS", 883.67m }
        };
    }

    //public decimal GetRate(string currency1, string currency2)
    //{
    //    currency1 = currency1.ToUpper();
    //    currency2 = currency2.ToUpper();

    //    return _rate[currency1 + currency2];
    //}

    public decimal GetRateToUsd(string currency)
    {
        currency = currency.ToUpper();

        return _rate[currency + "USD"];
    }

    public decimal GetRateFromUsd(string currency)
    {
        currency = currency.ToUpper();

        return _rate["USD" + currency];
    }
}
namespace TransactionStore.Core.Data;

public class DictionaryOfCoefficients
{
    private readonly Dictionary<string, decimal> _rate;

    public DictionaryOfCoefficients()
    {
        _rate = new Dictionary<string, decimal>()
        {
            { "RUBUSD", 0.011m },
            { "RUBEUR", 0.010m },
            { "RUBJPY", 1.71m },
            { "RUBCNY", 0.079m },
            { "RUBRSD", 1.18m },
            { "RUBBGN", 0.020m },
            { "RUBARS", 9.72m },

            { "USDRUB", 90.89m },
            { "USDEUR", 0.92m },
            { "USDJPY", 155.66m },
            { "USDCNY", 7.22m },
            { "USDRSD", 107.50m },
            { "USDBGN", 1.80m },
            { "USDARS", 883.67m },

            { "EURRUB", 98.80m },
            { "EURUSD", 1.09m },
            { "EURJPY", 169.16m },
            { "EURCNY", 7.85m },
            { "EURRSD", 116.86m },
            { "EURBGN", 1.95m },
            { "EURARS", 960.55m },

            { "JPYRUB", 0.58m },
            { "JPYUSD", 0.0064m },
            { "JPYEUR", 0.0059m },
            { "JPYCNY", 0.046m },
            { "JPYRSD", 0.69m },
            { "JPYBGN", 0.012m },
            { "JPYARS", 5.68m },

            { "CNYRUB", 12.83m },
            { "CNYUSD", 0.14m },
            { "CNYEUR", 0.13m },
            { "CNYJPY", 21.95m },
            { "CNYRSD", 15.16m },
            { "CNYBGN", 0.25m },
            { "CNYARS", 124.61m },

            { "RSDRUB", 0.85m },
            { "RSDUSD", 0.0093m },
            { "RSDEUR", 0.0086m },
            { "RSDJPY", 1.45m },
            { "RSDCNY", 0.067m },
            { "RSDBGN", 0.017m },
            { "RSDARS", 8.22m },

            { "BGNRUB", 50.63m },
            { "BGNUSD", 0.56m },
            { "BGNEUR", 0.51m },
            { "BGNJPY", 86.71m },
            { "BGNCNY", 4.02m },
            { "BGNRSD", 59.89m },
            { "BGNARS", 492.16m },

            { "ARSRUB", 0.10m },
            { "ARSUSD", 0.0011m },
            { "ARSEUR", 0.0010m },
            { "ARSJPY", 0.18m },
            { "ARSCNY", 0.0082m },
            { "ARSRSD", 0.12m },
            { "ARSBGN", 0.0020m }
        };
    }

    public decimal GetRate(string currency1, string currency2)
    {
        return _rate[currency1 + currency2];
    }
}
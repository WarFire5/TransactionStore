namespace TransactionStore.Core.Data;

public interface ICurrencyRatesProvider
{
    Task<decimal> ConvertFirstCurrencyToUsdAsync(Enum currencyEnum);
    Task<decimal> ConvertUsdToSecondCurrencyAsync(Enum currencyEnum);
}
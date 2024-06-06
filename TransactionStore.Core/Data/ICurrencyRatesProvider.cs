namespace TransactionStore.Core.Data;

public interface ICurrencyRatesProvider
{
    decimal ConvertFirstCurrencyToUsd(Enum currencyEnum);
    decimal ConvertUsdToSecondCurrency(Enum currencyEnum);
}
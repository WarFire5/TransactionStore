﻿namespace TransactionStore.Core.Data;

public interface ICurrencyRatesProvider
{
    decimal ConvertFirstCurrencyToUsd(Enum currencyType);
    decimal ConvertUsdToSecondCurrency(Enum currencyType);
}
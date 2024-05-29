﻿using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.DataLayer.Tests;

public static class TransactionsRepositoryTestData
{
    public static List<TransactionDto> GetFakeTransactionDtoList() =>
        [
        new()
        {
            AccountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1"),
            CurrencyType = CurrencyType.RUB,
            Amount = 1000,
        },
        new()
        {
            AccountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1"),
            CurrencyType = CurrencyType.RUB,
            Amount = -1000,
        },
        new()
        {
            AccountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1"),
            CurrencyType = CurrencyType.RUB,
            Amount = 500,
        }
        ];

    public static List<TransactionDto> GetFakeFullTransactionDtoList() =>
       [
       new()
       {
           Id = new Guid("550df504-032e-4ef7-aee2-53cf66e4d0c8"),
           AccountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa3"),
           CurrencyType = CurrencyType.RUB,
           TransactionType = TransactionType.Deposit,
           Amount = 1000,
       },
       new()
       {
           Id = new Guid("550df504-032e-4ef7-aee2-53cf66e4d0c8"),
           AccountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa3"),
           CurrencyType = CurrencyType.RUB,
           TransactionType = TransactionType.Withdraw,
           Amount = 1000,
       },
       new()
       {
           Id = new Guid("550df504-032e-4ef7-aee2-53cf66e4d0c8"),
           AccountId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa3"),
           CurrencyType = CurrencyType.RUB,
           TransactionType = TransactionType.Transfer,
           Amount = 500,
       }
       ];
}
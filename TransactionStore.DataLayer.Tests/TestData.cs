using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.DataLayer.Tests;

public static class TestData
{
    public static List<TransactionDto> GetFakeTransactionDtoList() =>
        [
        new()
        {
            AccountId = new Guid("08006bdf-86de-492a-8b09-5b4833acadea"),
            CurrencyType = CurrencyType.RUB,
            Amount = 1000,
        },
        new()
        {
            AccountId = new Guid("08006bdf-86de-492a-8b09-5b4833acadea"),
            CurrencyType = CurrencyType.RUB,
            Amount = -1000,
        },
        new()
        {
            AccountId = new Guid("08006bdf-86de-492a-8b09-5b4833acadea"),
            CurrencyType = CurrencyType.RUB,
            Amount = 500,
        }
        ];
}

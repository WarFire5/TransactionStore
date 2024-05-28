using TransactionStore.Core.DTOs;
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
}

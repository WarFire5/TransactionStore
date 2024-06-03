using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;

namespace TransactionStore.Business.Tests;

public static class TransactionsServiceTestData
{
    public static DepositWithdrawRequest GetValidDepositRequest(Guid accountId) => new()
    {
        AccountId = accountId,
        CurrencyType = CurrencyType.USD,
        Amount = 100
    };

    public static DepositWithdrawRequest GetValidWithdrawRequest(Guid accountId) => new()
    {
        AccountId = accountId,
        CurrencyType = CurrencyType.USD,
        Amount = 100
    };

    public static DepositWithdrawRequest GetDepositWithdrawRequest() => new()
    {
        AccountId = Guid.NewGuid(),
        CurrencyType = CurrencyType.USD,
        Amount = 100
    };

    public static TransferRequest GetValidWithdrawRequest() => new()
    {
        AccountFromId = Guid.NewGuid(),
        CurrencyFromType = CurrencyType.USD,
        Amount = 100
    };

    public static TransferRequest GetValidTransferRequest() => new()
    {
        AccountFromId = Guid.NewGuid(),
        AccountToId = Guid.NewGuid(),
        CurrencyFromType = CurrencyType.USD,
        CurrencyToType = CurrencyType.EUR,
        Amount = 100
    };

    public static TransferRequest GetTransferRequest() => new()
    {
        AccountFromId = Guid.NewGuid(),
        AccountToId = Guid.NewGuid(),
        CurrencyFromType = CurrencyType.RUB,
        CurrencyToType = CurrencyType.EUR,
        Amount = 100
    };
}
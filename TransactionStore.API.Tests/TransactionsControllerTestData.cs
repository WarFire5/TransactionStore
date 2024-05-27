using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;

public static class TransactionsControllerTestData
{
    public static DepositWithdrawRequest GetDepositRequest()
    {
        return new DepositWithdrawRequest
        {
            AccountId = Guid.NewGuid(),
            CurrencyType = CurrencyType.USD,
            Amount = 100
        };
    }

    public static DepositWithdrawRequest GetWithdrawRequest()
    {
        return new DepositWithdrawRequest
        {
            AccountId = Guid.NewGuid(),
            CurrencyType = CurrencyType.USD,
            Amount = 100
        };
    }

    public static TransferRequest GetTransferRequest()
    {
        return new TransferRequest
        {
            AccountFromId = Guid.NewGuid(),
            AccountToId = Guid.NewGuid(),
            CurrencyFromType = CurrencyType.RUB,
            CurrencyToType = CurrencyType.EUR,
            Amount = 100
        };
    }
}
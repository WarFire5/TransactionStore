using TransactionStore.Core.Data;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;

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

    public static DepositWithdrawRequest GetInvalidDepositWithdrawRequest() => new()
    {
        AccountId = Guid.NewGuid(),
        CurrencyType = CurrencyType.USD,
        Amount = 0
    };

    public static TransferRequest GetValidTransferRequest() => new()
    {
        AccountFromId = Guid.NewGuid(),
        AccountToId = Guid.NewGuid(),
        CurrencyFromType = CurrencyType.USD,
        CurrencyToType = CurrencyType.EUR,
        Amount = 100
    };

    public static TransferRequest GetInvalidTransferRequest() => new()
    {
        AccountFromId = Guid.NewGuid(),
        AccountToId = Guid.NewGuid(),
        CurrencyFromType = CurrencyType.RUB,
        CurrencyToType = CurrencyType.EUR,
        Amount = 0
    };

    public static TransactionDto CreateExpectedWithdrawTransaction(TransferRequest request)
    {
        return new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyFromType,
            Amount = request.Amount * -1
        };
    }

    public static TransactionDto CreateExpectedDepositTransaction(TransferRequest request)
    {
        var currencyRatesProvider = new CurrencyRatesProvider();
        var rateToUSD = currencyRatesProvider.ConvertFirstCurrencyToUsd(request.CurrencyFromType);
        var amountUsd = request.Amount * rateToUSD;
        var rateFromUsd = currencyRatesProvider.ConvertUsdToSecondCurrency(request.CurrencyToType);

        return new TransactionDto
        {
            AccountId = request.AccountToId,
            TransactionType = TransactionType.Transfer,
            CurrencyType = request.CurrencyToType,
            Amount = amountUsd * rateFromUsd
        };
    }

    public static List<TransactionDto> GetTransactions(Guid transactionId) =>
        [
            new TransactionDto { Id = transactionId, AccountId = Guid.NewGuid(), Amount = 100, CurrencyType = CurrencyType.USD, TransactionType = TransactionType.Deposit },
            new TransactionDto { Id = transactionId, AccountId = Guid.NewGuid(), Amount = 50, CurrencyType = CurrencyType.EUR, TransactionType = TransactionType.Withdraw }
        ];

    public static List<TransactionDto> GetEmptyTransactions() => [];

    public static List<TransactionDto> GetTransactionsByAccountId(Guid accountId) =>
        [
            new TransactionDto { Id = Guid.NewGuid(), AccountId = accountId, Amount = 100, CurrencyType = CurrencyType.USD, TransactionType = TransactionType.Deposit },
            new TransactionDto { Id = Guid.NewGuid(), AccountId = accountId, Amount = 50, CurrencyType = CurrencyType.EUR, TransactionType = TransactionType.Withdraw }
        ];
}
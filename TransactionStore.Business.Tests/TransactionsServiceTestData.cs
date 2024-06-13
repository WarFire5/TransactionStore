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
        CurrencyType = Currency.USD,
        Amount = 100
    };

    public static DepositWithdrawRequest GetValidWithdrawRequest(Guid accountId) => new()
    {
        AccountId = accountId,
        CurrencyType = Currency.USD,
        Amount = 100
    };

    public static DepositWithdrawRequest GetInvalidDepositWithdrawRequest() => new()
    {
        AccountId = Guid.NewGuid(),
        CurrencyType = Currency.USD,
        Amount = 0
    };

    public static TransferRequest GetValidTransferRequest() => new()
    {
        AccountFromId = Guid.NewGuid(),
        AccountToId = Guid.NewGuid(),
        CurrencyFromType = Currency.USD,
        CurrencyToType = Currency.EUR,
        Amount = 100
    };

    public static TransferRequest GetInvalidTransferRequest() => new()
    {
        AccountFromId = Guid.NewGuid(),
        AccountToId = Guid.NewGuid(),
        CurrencyFromType = Currency.RUB,
        CurrencyToType = Currency.EUR,
        Amount = 0
    };

    public static TransactionDto CreateExpectedWithdrawTransaction(TransferRequest request, decimal commissionAmount)
    {
        return new TransactionDto
        {
            AccountId = request.AccountFromId,
            TransactionType = TransactionType.Transfer,
            Amount = request.Amount * -1 - commissionAmount
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
            Amount = amountUsd * rateFromUsd
        };
    }

    public static List<TransactionDto> GetTransactions(Guid transactionId) =>
        [
            new TransactionDto { Id = transactionId, AccountId = Guid.NewGuid(), Amount = 100, TransactionType = TransactionType.Deposit },
            new TransactionDto { Id = transactionId, AccountId = Guid.NewGuid(), Amount = 50, TransactionType = TransactionType.Withdraw }
        ];

    public static List<TransactionDto> GetEmptyTransactions() => [];

    public static List<TransactionDto> GetTransactionsByAccountId(Guid accountId) =>
        [
            new TransactionDto { Id = Guid.NewGuid(), AccountId = accountId, Amount = 100, TransactionType = TransactionType.Deposit },
            new TransactionDto { Id = Guid.NewGuid(), AccountId = accountId, Amount = 50, TransactionType = TransactionType.Withdraw }
        ];
}
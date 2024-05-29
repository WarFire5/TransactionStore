using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.Business.Services;

public interface ITransactionsService
{
    AccountBalanceResponse GetBalanceByAccountId(Guid id);
    Guid AddDepositWithdrawTransaction(TransactionType transactionType, DepositWithdrawRequest request);
    void AddTransferTransaction(TransferRequest request);
}
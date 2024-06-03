using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.Business.Services;

public interface ITransactionsService
{
    AccountBalanceResponse GetBalanceByAccountId(Guid id);
    List<TransactionResponse> GetTransactionsByAccountId(Guid id);
    List<TransactionWithAccountIdResponse> GetTransactionsByLeadId(Guid id);
    Guid AddDepositWithdrawTransaction(TransactionType transactionType, DepositWithdrawRequest request);
    void AddTransferTransaction(TransferRequest request);
}
using TransactionStore.Core.Models.Transactions.Requests;

namespace TransactionStore.Business.Services;

public interface ITransactionsService
{
    Guid AddDepositTransaction(DepositWithdrawRequest request);
    Guid AddWithdrawTransaction(DepositWithdrawRequest request);
    void AddTransferTransaction(TransferRequest request);
    // TransactionDto GetTransactionById(Guid id);
    // List<TransactionResponse> GetTransactionsByAccountId(Guid accountId);
}
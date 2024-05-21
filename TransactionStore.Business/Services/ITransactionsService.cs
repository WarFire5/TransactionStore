using TransactionStore.Core.DTOs;
using TransactionStore.Core.Models.Transactions.Requests;
using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.Business.Services;

public interface ITransactionsService
{
    Guid AddDepositTransaction(DepositWithdrawRequest request);
    Guid AddWithdrawTransaction(DepositWithdrawRequest request);
    void AddTransferTransaction(TransferRequest request);
    // TransactionDto GetTransactionById(Guid id);
    // List<TransactionResponse> GetTransactionsByAccountId(Guid accountId);
}

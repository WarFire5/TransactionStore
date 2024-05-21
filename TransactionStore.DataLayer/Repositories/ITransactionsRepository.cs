using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public interface ITransactionsRepository
{   
    Guid AddDepositTransaction(TransactionDto transaction);
    Guid AddWithdrawTransaction(TransactionDto transaction);
    void AddTransferTransaction(TransactionDto transferWithdraw, TransactionDto transferDeposit);
    // TransactionDto GetTransactionById(Guid id);
    // List<TransactionDto> GetTransactionsByAccountId(Guid accountId);
}
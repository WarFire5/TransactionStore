using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public interface ITransactionsRepository
{
    List<TransactionDto> GetBalanceByAccountId(Guid id);
    List<TransactionDto> GetTransactionsByAccountId(Guid id);
    Guid AddDepositWithdrawTransaction(TransactionDto transaction);
    void AddTransferTransaction(TransactionDto transferWithdraw, TransactionDto transferDeposit);
    // TransactionDto GetTransactionById(Guid id);
}
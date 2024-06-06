using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public interface ITransactionsRepository
{
    Task<List<TransactionDto>> GetTransactionsByAccountIdAsync(Guid id);
    Task<List<TransactionDto>> GetTransactionsByIdAsync(Guid id);
    Task<Guid> AddDepositWithdrawTransactionAsync(TransactionDto transaction);
    Task AddTransferTransactionAsync(TransactionDto transferWithdraw, TransactionDto transferDeposit);
}
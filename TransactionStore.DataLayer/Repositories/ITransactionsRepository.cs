using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public interface ITransactionsRepository
{
    Task<Guid> AddDepositWithdrawTransactionAsync(TransactionDto transaction);
    Task AddTransferTransactionAsync(TransactionDto transferWithdraw, TransactionDto transferDeposit);
    Task<List<TransactionDto>> GetTransactionByIdAsync(Guid id);
    Task<List<TransactionDto>> GetTransactionsByAccountIdAsync(Guid id);
}
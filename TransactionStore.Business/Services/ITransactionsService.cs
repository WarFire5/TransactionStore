using TransactionStore.Core.Enums;
using TransactionStore.Core.Models.Requests;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.Business.Services;

public interface ITransactionsService
{
    Task<AccountBalanceResponse> GetBalanceByAccountIdAsync(Guid id);
    Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id);
    Task<List<TransactionWithAccountIdResponse>> GetTransactionsByLeadIdAsync(Guid id);
    Task<Guid> AddDepositWithdrawTransactionAsync(TransactionType transactionType, DepositWithdrawRequest request);
    Task AddTransferTransactionAsync(TransferRequest request);
}
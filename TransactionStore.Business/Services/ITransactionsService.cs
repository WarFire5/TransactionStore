using TransactionStore.Core.Models.Transactions.Responses;

namespace TransactionStore.Business.Services;

public interface ITransactionsService
{
    AccountBalanceResponse GetBalanceByAccountId(Guid id);
}

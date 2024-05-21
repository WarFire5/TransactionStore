using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public interface ITransactionsRepository
{
    List<TransactionDto> GetBalanceByAccountId(Guid id);
}
using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public interface ITransactionsRepository
{
    List<TransactionDto> GetTransactionsByAccountId(Guid id);
    List<TransactionDto> GetTransactionsByLeadId(Guid id);
}
using Serilog;
using TransactionStore.Core.Constants.Exceptions;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Exceptions;

namespace TransactionStore.DataLayer.Repositories;

public class TransactionsRepository : BaseRepository, ITransactionsRepository
{
    private readonly ILogger _logger = Log.ForContext<TransactionsRepository>();

    public TransactionsRepository(TransactionStoreContext context) : base(context)
    {
        if (!_ctx.Database.CanConnect())
        {
            throw new ServiceUnavailableException(RepositoryExceptions.ServiceUnavailable);
        }
    }

    public List<TransactionDto> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information($"Ищем в базе транзакции аккаунта {id}");
        return _ctx.Transactions.Where(t => t.AccountId == id).ToList();
    }

    public List<TransactionDto> GetTransactionsByLeadId(Guid id)
    {
        _logger.Information($"Ищем в базе транзакции лида {id}");
        return _ctx.Transactions.Where(t => t.Id == id).ToList();
    }
}
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

    public List<TransactionDto> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Ищем в базе транзакции аккаунта с Id {id}.");
        return _ctx.Transactions.Where(t => t.AccountId == id).ToList();
    }

    public List<TransactionDto> GetTransactionsByAccountId(Guid id)
    {
        _logger.Information($"Ищем в базе транзакции аккаунта с Id {id}.");
        return _ctx.Transactions.Where(t => t.AccountId == id).ToList();
    }

    public List<TransactionDto> GetTransactionsByLeadId(Guid id)
    {
        _logger.Information($"Ищем в базе транзакции лида {id}");
        return _ctx.Transactions.Where(t => t.Id == id).ToList();
    }

    public Guid AddDepositWithdrawTransaction(TransactionDto transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null. / Транзакция не может быть нулевой.");
        }

        _logger.Information($"Recording the transaction in the database. / Записываем транзакцию в базу.");
        _ctx.Transactions.Add(transaction);
        _ctx.SaveChanges();

        _logger.Information($"Returning the Id {transaction.Id} of the added transaction. / Возвращаем Id {transaction.Id} добавленной транзакции.");
        return transaction.Id;
    }

    public void AddTransferTransaction(TransactionDto transferWithdraw, TransactionDto transferDeposit)
    {
        if (transferWithdraw == null)
        {
            throw new ArgumentNullException(nameof(transferWithdraw), "Transfer-withdraw transaction cannot be null. / Транзакция на перевод-снятие не может быть нулевой.");
        }

        if (transferDeposit == null)
        {
            throw new ArgumentNullException(nameof(transferDeposit), "Transfer-deposit transaction cannot be null. / Транзакция на перевод-пополнение не может быть нулевой.");
        }

        _logger.Information($"Recording the transfer-transactions in the database. / Записываем транзакции в базу.");
        _ctx.Transactions.Add(transferWithdraw);
        _ctx.Transactions.Add(transferDeposit);
        _ctx.SaveChanges();
    }
}
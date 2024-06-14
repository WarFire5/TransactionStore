using Microsoft.EntityFrameworkCore;
using Serilog;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Exceptions;
using TransactionStore.Core.Models.Responses;

namespace TransactionStore.DataLayer.Repositories;

public class TransactionsRepository : BaseRepository, ITransactionsRepository
{
    private readonly ILogger _logger = Log.ForContext<TransactionsRepository>();

    public TransactionsRepository(TransactionStoreContext context) : base(context)
    {
        if (!_ctx.Database.CanConnect())
        {
            throw new ServiceUnavailableException("There is no connection to the database. / Нет соединения с базой данных.");
        }
    }

    public async Task<Guid> AddDepositWithdrawTransactionAsync(TransactionDto transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null. / Транзакция не может быть нулевой.");
        }

        _logger.Information($"Recording the transaction in the database. / Записываем транзакцию в базу.");
        _ctx.Transactions.Add(transaction);
        await _ctx.SaveChangesAsync();

        _logger.Information($"Returning the Id {transaction.Id} of the added transaction. / Возвращаем Id {transaction.Id} добавленной транзакции.");
        return transaction.Id;
    }

    public async Task<TransferGuidsResponse> AddTransferTransactionAsync(TransactionDto transferWithdraw, TransactionDto transferDeposit)
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
        await _ctx.Transactions.AddAsync(transferWithdraw);
        await _ctx.Transactions.AddAsync(transferDeposit);
        await _ctx.SaveChangesAsync();

        return new TransferGuidsResponse
        {
            TransferWithdrawId = transferWithdraw.Id,
            TransferDepositId = transferDeposit.Id
        };
    }

    public async Task<List<TransactionDto>> GetTransactionByIdAsync(Guid id)
    {
        _logger.Information($"Looking for transaction by Id {id} in the database. / Ищем в базе транзакцию по Id {id}.");
        var transaction = await _ctx.Transactions.AsNoTracking().Where(t => t.Id == id).FirstOrDefaultAsync();
        var transactionDateTime = transaction.Date;
        var accountId = transaction.AccountId;

        return await _ctx.Transactions.AsNoTracking().Where(t => t.Date == transactionDateTime).ToListAsync();
    }

    public async Task<List<TransactionDto>> GetTransactionsByAccountIdAsync(Guid id)
    {
        _logger.Information($"Looking for transactions by accountId {id} in the database. / Ищем в базе транзакции аккаунта с Id {id}.");
        return await _ctx.Transactions.AsNoTracking().Where(t => t.AccountId == id).ToListAsync();
    }
}
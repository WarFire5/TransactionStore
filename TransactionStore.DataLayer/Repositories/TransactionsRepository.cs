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
            _logger.Error("Throwing an error if there is no connection to the database. / Выдача ошибки, если нет соединения с базой данных.");
            throw new ServiceUnavailableException("There is no connection to the database. / Нет соединения с базой данных.");
        }
    }

    public async Task<Guid> AddDepositWithdrawTransactionAsync(TransactionDto transaction)
    {
        if (transaction == null)
        {
            _logger.Information("Throwing an error if the transaction is null. / Выдача ошибки, если транзакция равна null.");
            throw new Core.Exceptions.ArgumentNullException("Transaction cannot be null. / Транзакция не может быть нулевой.");
        }

        _logger.Information($"Saving the transaction in the database. / Сохранение транзакции в базе.");
        _ctx.Transactions.Add(transaction);
        await _ctx.SaveChangesAsync();

        _logger.Information($"Returning the Id {transaction.Id} of the added transaction. / Возвращаем Id {transaction.Id} добавленной транзакции.");
        return transaction.Id;
    }

    public async Task<TransferGuidsResponse> AddTransferTransactionAsync(TransactionDto transferWithdraw, TransactionDto transferDeposit)
    {
        if (transferWithdraw == null)
        {
            _logger.Information("Throwing an error if the transfer-withdraw transaction is null. / Выдача ошибки, если транзакция на перевод-снятие равна null.");
            throw new Core.Exceptions.ArgumentNullException("Transfer-withdraw transaction cannot be null. / Транзакция на перевод-снятие не может быть нулевой.");
        }

        if (transferDeposit == null)
        {
            _logger.Information("Throwing an error if the transfer-deposit is null. / Выдача ошибки, если транзакция на перевод-пополнение равна null.");
            throw new Core.Exceptions.ArgumentNullException("Transfer-deposit transaction cannot be null. / Транзакция на перевод-пополнение не может быть нулевой.");
        }

        _logger.Information($"Registering and saving the records of the transfer transaction in the database. / Регистрируем и сохраняем записи о трансферной транзакции в базе.");
        await _ctx.Transactions.AddAsync(transferWithdraw);
        await _ctx.Transactions.AddAsync(transferDeposit);
        await _ctx.SaveChangesAsync();

        _logger.Information($"Returning transfer-withdraw Id {transferWithdraw.Id} and transfer-deposit Id {transferDeposit.Id}. / Возвращаем Id {transferWithdraw.Id} перевода-cнятия и Id {transferDeposit.Id} перевода-пополнения.");
        return new TransferGuidsResponse
        {
            TransferWithdrawId = transferWithdraw.Id,
            TransferDepositId = transferDeposit.Id
        };
    }

    public async Task<List<TransactionDto>> GetTransactionByIdAsync(Guid id)
    {
        _logger.Information($"Looking for transaction by Id {id} in the database. / Ищем в базе транзакцию по Id {id}.");
        var transaction = await _ctx.Transactions.AsNoTracking().Where(t => t.Id == id).ToListAsync();

        if (transaction.Count == 0)
        {
            throw new NotFoundException($"Transaction with Id {id} not found. / Транзакция с Id {id} не найдена.");
        }

        if (transaction[0].TransactionType == Core.Enums.TransactionType.Transfer)
        {
            var transactionDateTime = transaction[0].Date;

            return await _ctx.Transactions.AsNoTracking().Where(t => t.Date == transactionDateTime).ToListAsync();
        }
        else
        {
            return transaction;
        }
    }

    public async Task<List<TransactionDto>> GetTransactionsByAccountIdAsync(Guid id)
    {
        _logger.Information($"Returning information about transactions of the account with Id {id}. / Возвращаем информацию о транзакциях аккаунта с Id {id}.");
        var transactions = await _ctx.Transactions.AsNoTracking().Where(t => t.AccountId == id).ToListAsync();

        if (transactions == null || transactions.Count == 0)
        {
            _logger.Information($"Throwing an error if transactions for account with Id {id} not found. / Транзакции для аккаунта с Id {id} не найдены.");
            throw new NotFoundException($"No transactions found for account with Id {id}. / Транзакции для аккаунта с Id {id} не найдены.");
        }

        return transactions;
    }
}
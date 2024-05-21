﻿using Serilog;
using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public class TransactionsRepository : BaseRepository, ITransactionsRepository
{
    private readonly ILogger _logger = Log.ForContext<TransactionsRepository>();

    public TransactionsRepository(TransactionStoreContext context) : base(context)
    {
    }

    public List<TransactionDto> GetBalanceByAccountId(Guid id)
    {
        _logger.Information($"Ищем в базе транзакции аккаунта {id}");
        return _ctx.Transactions.Where(t=>t.AccountId==id).ToList();
    }
}
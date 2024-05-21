using Serilog;
using TransactionStore.Core.DTOs;

namespace TransactionStore.DataLayer.Repositories;

public class TransactionsRepository : BaseRepository, ITransactionsRepository
{
    private readonly ILogger _logger = Log.ForContext<TransactionsRepository>();

    public TransactionsRepository(TransactionStoreContext context) : base(context)
    {
    }

    public Guid AddDepositTransaction(TransactionDto transaction)
    {
        _ctx.Transactions.Add(transaction);
        _ctx.SaveChanges();

        return transaction.Id;
    }

    public Guid AddWithdrawTransaction(TransactionDto transaction)
    {
        _ctx.Transactions.Add(transaction);
        _ctx.SaveChanges();

        return transaction.Id;
    }

    public void AddTransferTransaction(TransactionDto transferWithdraw, TransactionDto transferDeposit)
    {
        _ctx.Transactions.Add(transferWithdraw);
        _ctx.Transactions.Add(transferDeposit);
        _ctx.SaveChanges();
    }
}
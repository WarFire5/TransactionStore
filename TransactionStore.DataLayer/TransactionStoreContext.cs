using Microsoft.EntityFrameworkCore;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.DataLayer;

public class TransactionStoreContext : DbContext
{
    public DbSet<TransactionDto> Transactions { get; set; }

    public TransactionStoreContext(DbContextOptions<TransactionStoreContext> options) : base(options)
    {
    }

    public TransactionStoreContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<CurrencyType>();
        modelBuilder.HasPostgresEnum<CurrencyPairType>();
        modelBuilder.HasPostgresEnum<TransactionType>();
    }
}
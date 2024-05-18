using Microsoft.EntityFrameworkCore;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.DataLayer;

public class TransactionStoreContext : DbContext
{
    public DbSet<LeadDto> Leads { get; set; }
    public DbSet<AccountDto> Accounts { get; set; }
    public DbSet<TransactionDto> Transactions { get; set; }

    public TransactionStoreContext(DbContextOptions<TransactionStoreContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
              .Entity<AccountDto>()
              .HasOne(a => a.Lead)
              .WithMany(l => l.Accounts);

        modelBuilder
            .Entity<TransactionDto>()
            .HasOne(t => t.Account)
            //.HasOne(t => t.AccountTo)
            .WithMany(l => l.Transactions);

        modelBuilder.HasPostgresEnum<CurrencyType>();
        modelBuilder.HasPostgresEnum<CurrencyPairType>();
        modelBuilder.HasPostgresEnum<TransactionType>();
    }
}
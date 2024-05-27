using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.DataLayer;

public class TransactionStoreContext : DbContext
{
    public virtual DbSet<TransactionDto> Transactions { get; set; }

    public TransactionStoreContext(DbContextOptions<TransactionStoreContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<CurrencyType>();
        modelBuilder.HasPostgresEnum<TransactionType>();

        modelBuilder.Entity<TransactionDto>(ConfigureTransactionDto);
    }

    private void ConfigureTransactionDto(EntityTypeBuilder<TransactionDto> builder)
    {
        builder.Property(t => t.Amount)
               .HasColumnType("decimal(11, 4)");

        builder.Property(t => t.Date)
               .HasDefaultValueSql("NOW()");
    }
}
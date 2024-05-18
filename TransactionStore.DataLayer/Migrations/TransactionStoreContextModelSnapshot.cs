﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TransactionStore.Core.Enums;
using TransactionStore.DataLayer;

#nullable disable

namespace TransactionStore.DataLayer.Migrations
{
    [DbContext(typeof(TransactionStoreContext))]
    partial class TransactionStoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "currency_pair_type", new[] { "unknown", "rubusd", "rubeur", "rubjpy", "rubcny", "rubrsd", "rubbgn", "rubars", "usdrub", "usdeur", "usdjpy", "usdcny", "usdrsd", "usdbgn", "usdars", "eurrub", "eurusd", "eurjpy", "eurcny", "eurrsd", "eurbgn", "eurars", "jpyrub", "jpyusd", "jpyeur", "jpycny", "jpyrsd", "jpybgn", "jpyars", "cnyrub", "cnyusd", "cnyeur", "cnyjpy", "cnyrsd", "cnybgn", "cnyars", "rsdrub", "rsdusd", "rsdeur", "rsdjpy", "rsdcny", "rsdbgn", "rsdars", "bgnrub", "bgnusd", "bgneur", "bgnjpy", "bgncny", "bgnrsd", "bgnars", "arsrub", "arsusd", "arseur", "arsjpy", "arscny", "arsrsd", "arsbgn" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "currency_type", new[] { "unknown", "rub", "usd", "eur", "jpy", "cny", "rsd", "bgn", "ars" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "transaction_type", new[] { "unknown", "deposit", "withdraw", "transfer_withdraw", "transfer_deposit", "transfer" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TransactionStore.Core.DTOs.AccountDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AccountName")
                        .HasColumnType("text")
                        .HasColumnName("account_name");

                    b.Property<int>("Balance")
                        .HasColumnType("integer")
                        .HasColumnName("balance");

                    b.Property<CurrencyType>("CurrencyType")
                        .HasColumnType("currency_type")
                        .HasColumnName("currency_type");

                    b.Property<Guid?>("LeadId")
                        .HasColumnType("uuid")
                        .HasColumnName("lead_id");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_accounts");

                    b.HasIndex("LeadId")
                        .HasDatabaseName("ix_accounts_lead_id");

                    b.ToTable("accounts", (string)null);
                });

            modelBuilder.Entity("TransactionStore.Core.DTOs.LeadDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.HasKey("Id")
                        .HasName("pk_leads");

                    b.ToTable("leads", (string)null);
                });

            modelBuilder.Entity("TransactionStore.Core.DTOs.TransactionDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("AccountId")
                        .HasColumnType("uuid")
                        .HasColumnName("account_id");

                    b.Property<int>("Amount")
                        .HasColumnType("integer")
                        .HasColumnName("amount");

                    b.Property<CurrencyType>("CurrencyType")
                        .HasColumnType("currency_type")
                        .HasColumnName("currency_type");

                    b.Property<TransactionType>("TransactionType")
                        .HasColumnType("transaction_type")
                        .HasColumnName("transaction_type");

                    b.Property<DateTime>("time")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time");

                    b.HasKey("Id")
                        .HasName("pk_transactions");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_transactions_account_id");

                    b.ToTable("transactions", (string)null);
                });

            modelBuilder.Entity("TransactionStore.Core.DTOs.AccountDto", b =>
                {
                    b.HasOne("TransactionStore.Core.DTOs.LeadDto", "Lead")
                        .WithMany("Accounts")
                        .HasForeignKey("LeadId")
                        .HasConstraintName("fk_accounts_leads_lead_id");

                    b.Navigation("Lead");
                });

            modelBuilder.Entity("TransactionStore.Core.DTOs.TransactionDto", b =>
                {
                    b.HasOne("TransactionStore.Core.DTOs.AccountDto", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .HasConstraintName("fk_transactions_accounts_account_id");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("TransactionStore.Core.DTOs.AccountDto", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("TransactionStore.Core.DTOs.LeadDto", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}

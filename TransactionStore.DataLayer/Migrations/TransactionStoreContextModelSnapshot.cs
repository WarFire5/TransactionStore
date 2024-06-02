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

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "currency_type", new[] { "unknown", "rub", "usd", "eur", "jpy", "cny", "rsd", "bgn", "ars" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "transaction_type", new[] { "unknown", "deposit", "withdraw", "transfer" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TransactionStore.Core.DTOs.TransactionDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid")
                        .HasColumnName("account_id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(11, 4)")
                        .HasColumnName("amount");

                    b.Property<CurrencyType>("CurrencyType")
                        .HasColumnType("currency_type")
                        .HasColumnName("currency_type");

                    b.Property<DateTime>("Date")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date")
                        .HasDefaultValueSql("NOW()");

                    b.Property<TransactionType>("TransactionType")
                        .HasColumnType("transaction_type")
                        .HasColumnName("transaction_type");

                    b.HasKey("Id")
                        .HasName("pk_transactions");

                    b.ToTable("transactions", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}

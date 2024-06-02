using System;
using Microsoft.EntityFrameworkCore.Migrations;
using TransactionStore.Core.Enums;

#nullable disable

namespace TransactionStore.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class CreatedDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:currency_type", "unknown,rub,usd,eur,jpy,cny,rsd,bgn,ars")
                .Annotation("Npgsql:Enum:transaction_type", "unknown,deposit,withdraw,transfer");

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_type = table.Column<TransactionType>(type: "transaction_type", nullable: false),
                    currency_type = table.Column<CurrencyType>(type: "currency_type", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(11,4)", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transactions");
        }
    }
}

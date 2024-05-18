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
                .Annotation("Npgsql:Enum:currency_pair_type", "unknown,rubusd,rubeur,rubjpy,rubcny,rubrsd,rubbgn,rubars,usdrub,usdeur,usdjpy,usdcny,usdrsd,usdbgn,usdars,eurrub,eurusd,eurjpy,eurcny,eurrsd,eurbgn,eurars,jpyrub,jpyusd,jpyeur,jpycny,jpyrsd,jpybgn,jpyars,cnyrub,cnyusd,cnyeur,cnyjpy,cnyrsd,cnybgn,cnyars,rsdrub,rsdusd,rsdeur,rsdjpy,rsdcny,rsdbgn,rsdars,bgnrub,bgnusd,bgneur,bgnjpy,bgncny,bgnrsd,bgnars,arsrub,arsusd,arseur,arsjpy,arscny,arsrsd,arsbgn")
                .Annotation("Npgsql:Enum:currency_type", "unknown,rub,usd,eur,jpy,cny,rsd,bgn,ars")
                .Annotation("Npgsql:Enum:transaction_type", "unknown,deposit,withdraw,transfer_withdraw,transfer_deposit,transfer");

            migrationBuilder.CreateTable(
                name: "leads",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lead_id = table.Column<Guid>(type: "uuid", nullable: true),
                    account_name = table.Column<string>(type: "text", nullable: true),
                    currency_type = table.Column<CurrencyType>(type: "currency_type", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_accounts_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    transaction_type = table.Column<TransactionType>(type: "transaction_type", nullable: false),
                    currency_type = table.Column<CurrencyType>(type: "currency_type", nullable: false),
                    currency_rate = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_transactions_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_lead_id",
                table: "accounts",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_account_id",
                table: "transactions",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "leads");
        }
    }
}

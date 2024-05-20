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
                .Annotation("Npgsql:Enum:transaction_type", "unknown,deposit,withdraw,transfer");

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accountid = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_type = table.Column<TransactionType>(type: "transaction_type", nullable: false),
                    currency_type = table.Column<CurrencyType>(type: "currency_type", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionStore.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Added_currencies_rates_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies_rates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency = table.Column<int>(type: "integer", nullable: false),
                    rate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies_rates", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currencies_rates");
        }
    }
}

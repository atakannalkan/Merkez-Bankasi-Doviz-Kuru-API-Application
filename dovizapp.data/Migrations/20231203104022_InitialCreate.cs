using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dovizapp.data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    ForexBuying = table.Column<double>(type: "float", nullable: true),
                    ForexSelling = table.Column<double>(type: "float", nullable: true),
                    BanknoteBuying = table.Column<double>(type: "float", nullable: true),
                    BanknoteSelling = table.Column<double>(type: "float", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyLogs",
                columns: table => new
                {
                    CurrencyLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    ForexBuying = table.Column<double>(type: "float", nullable: true),
                    ForexSelling = table.Column<double>(type: "float", nullable: true),
                    BanknoteBuying = table.Column<double>(type: "float", nullable: true),
                    BanknoteSelling = table.Column<double>(type: "float", nullable: true),
                    LogAddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyLogs", x => x.CurrencyLogId);
                    table.ForeignKey(
                        name: "FK_CurrencyLogs_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CurrencyCode",
                table: "Currencies",
                column: "CurrencyCode",
                unique: true,
                filter: "[CurrencyCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyLogs_CurrencyId",
                table: "CurrencyLogs",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyLogs");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoPriceTracker.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CryptoPriceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CryptoAssetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoPriceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CryptoPriceHistories_CryptoAssets_CryptoAssetId",
                        column: x => x.CryptoAssetId,
                        principalTable: "CryptoAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CryptoAssets",
                columns: new[] { "Id", "ExternalId", "Name", "Symbol" },
                values: new object[] { 1, "bitcoin", "Bitcoin", "BTC" });

            migrationBuilder.InsertData(
                table: "CryptoAssets",
                columns: new[] { "Id", "ExternalId", "Name", "Symbol" },
                values: new object[] { 2, "ethereum", "Ethereum", "ETH" });

            migrationBuilder.CreateIndex(
                name: "IX_CryptoPriceHistories_CryptoAssetId",
                table: "CryptoPriceHistories",
                column: "CryptoAssetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoPriceHistories");

            migrationBuilder.DropTable(
                name: "CryptoAssets");
        }
    }
}

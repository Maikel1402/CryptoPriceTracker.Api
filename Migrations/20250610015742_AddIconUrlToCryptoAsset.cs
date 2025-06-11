using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoPriceTracker.Api.Migrations
{
    public partial class AddIconUrlToCryptoAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "CryptoAssets",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "CryptoAssets");
        }
    }
}

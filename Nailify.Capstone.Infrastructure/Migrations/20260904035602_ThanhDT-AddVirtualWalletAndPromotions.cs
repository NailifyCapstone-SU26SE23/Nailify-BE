using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddVirtualWalletAndPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyTransactions_LoyaltyTiers_LoyaltyTierIdAtTime",
                table: "LoyaltyTransactions");

            migrationBuilder.DropIndex(
                name: "IX_LoyaltyTransactions_LoyaltyTierIdAtTime",
                table: "LoyaltyTransactions");

            migrationBuilder.DropColumn(
                name: "LoyaltyTierIdAtTime",
                table: "LoyaltyTransactions");

            migrationBuilder.AddColumn<int>(
                name: "PointsRequired",
                table: "Promotions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LoyaltyTransactions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsRequired",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LoyaltyTransactions");

            migrationBuilder.AddColumn<int>(
                name: "LoyaltyTierIdAtTime",
                table: "LoyaltyTransactions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_LoyaltyTierIdAtTime",
                table: "LoyaltyTransactions",
                column: "LoyaltyTierIdAtTime");

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyTransactions_LoyaltyTiers_LoyaltyTierIdAtTime",
                table: "LoyaltyTransactions",
                column: "LoyaltyTierIdAtTime",
                principalTable: "LoyaltyTiers",
                principalColumn: "LoyaltyTierId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintFromLoyaltyTransactionBookingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoyaltyTransactions_BookingId",
                table: "LoyaltyTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_BookingId",
                table: "LoyaltyTransactions",
                column: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LoyaltyTransactions_BookingId",
                table: "LoyaltyTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_BookingId",
                table: "LoyaltyTransactions",
                column: "BookingId",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGRemoveNailVariantInCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerNails_NailVariants_BasedOnNailVariantId",
                table: "CustomerNails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerNails_BasedOnNailVariantId",
                table: "CustomerNails");

            migrationBuilder.DropColumn(
                name: "BasedOnNailVariantId",
                table: "CustomerNails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BasedOnNailVariantId",
                table: "CustomerNails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_BasedOnNailVariantId",
                table: "CustomerNails",
                column: "BasedOnNailVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerNails_NailVariants_BasedOnNailVariantId",
                table: "CustomerNails",
                column: "BasedOnNailVariantId",
                principalTable: "NailVariants",
                principalColumn: "NailVariantId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

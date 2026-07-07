using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGRemoveSalonInCustomerNail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalonId",
                table: "CustomerNails",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_SalonId",
                table: "CustomerNails",
                column: "SalonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerNails_Salons_SalonId",
                table: "CustomerNails",
                column: "SalonId",
                principalTable: "Salons",
                principalColumn: "SalonId");
        }
    }
}

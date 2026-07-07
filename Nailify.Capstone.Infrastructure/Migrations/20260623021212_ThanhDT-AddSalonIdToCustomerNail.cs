using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddSalonIdToCustomerNail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                principalColumn: "SalonId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerNails_Salons_SalonId",
                table: "CustomerNails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerNails_SalonId",
                table: "CustomerNails");

            migrationBuilder.DropColumn(
                name: "SalonId",
                table: "CustomerNails");
        }
    }
}

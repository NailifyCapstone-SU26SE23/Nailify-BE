using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGFixBookingItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_CustomerNails_CustomerNailId",
                table: "BookingItems");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_CustomerNailId",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "CustomerNailId",
                table: "BookingItems");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerNailRequestId",
                table: "BookingItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_CustomerNailRequestId",
                table: "BookingItems",
                column: "CustomerNailRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_CustomerNailRequests_CustomerNailRequestId",
                table: "BookingItems",
                column: "CustomerNailRequestId",
                principalTable: "CustomerNailRequests",
                principalColumn: "CustomerNailRequestId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_CustomerNailRequests_CustomerNailRequestId",
                table: "BookingItems");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_CustomerNailRequestId",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "CustomerNailRequestId",
                table: "BookingItems");

            migrationBuilder.AddColumn<int>(
                name: "CustomerNailId",
                table: "BookingItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_CustomerNailId",
                table: "BookingItems",
                column: "CustomerNailId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_CustomerNails_CustomerNailId",
                table: "BookingItems",
                column: "CustomerNailId",
                principalTable: "CustomerNails",
                principalColumn: "CustomerNailId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

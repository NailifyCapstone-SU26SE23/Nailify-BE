using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTSyncWaitlistItemWithBookingItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerNailRequestId",
                table: "WaitlistItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShapeMethodConfigId",
                table: "WaitlistItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistItems_CustomerNailRequestId",
                table: "WaitlistItems",
                column: "CustomerNailRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistItems_ShapeMethodConfigId",
                table: "WaitlistItems",
                column: "ShapeMethodConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitlistItems_CustomerNailRequests_CustomerNailRequestId",
                table: "WaitlistItems",
                column: "CustomerNailRequestId",
                principalTable: "CustomerNailRequests",
                principalColumn: "CustomerNailRequestId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WaitlistItems_ShapeMethodConfigs_ShapeMethodConfigId",
                table: "WaitlistItems",
                column: "ShapeMethodConfigId",
                principalTable: "ShapeMethodConfigs",
                principalColumn: "ShapeMethodConfigId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaitlistItems_CustomerNailRequests_CustomerNailRequestId",
                table: "WaitlistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WaitlistItems_ShapeMethodConfigs_ShapeMethodConfigId",
                table: "WaitlistItems");

            migrationBuilder.DropIndex(
                name: "IX_WaitlistItems_CustomerNailRequestId",
                table: "WaitlistItems");

            migrationBuilder.DropIndex(
                name: "IX_WaitlistItems_ShapeMethodConfigId",
                table: "WaitlistItems");

            migrationBuilder.DropColumn(
                name: "CustomerNailRequestId",
                table: "WaitlistItems");

            migrationBuilder.DropColumn(
                name: "ShapeMethodConfigId",
                table: "WaitlistItems");
        }
    }
}

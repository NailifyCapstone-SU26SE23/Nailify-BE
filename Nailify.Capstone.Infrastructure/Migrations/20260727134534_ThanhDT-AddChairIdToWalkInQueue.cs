using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddChairIdToWalkInQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChairId",
                table: "WalkInQueues",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalkInQueues_ChairId",
                table: "WalkInQueues",
                column: "ChairId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalkInQueues_Chairs_ChairId",
                table: "WalkInQueues",
                column: "ChairId",
                principalTable: "Chairs",
                principalColumn: "ChairId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalkInQueues_Chairs_ChairId",
                table: "WalkInQueues");

            migrationBuilder.DropIndex(
                name: "IX_WalkInQueues_ChairId",
                table: "WalkInQueues");

            migrationBuilder.DropColumn(
                name: "ChairId",
                table: "WalkInQueues");
        }
    }
}

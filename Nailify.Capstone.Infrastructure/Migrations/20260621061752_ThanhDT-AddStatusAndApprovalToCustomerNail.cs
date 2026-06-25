using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddStatusAndApprovalToCustomerNail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CustomerNails",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Active");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedArtistId",
                table: "CustomerNails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "CustomerNails",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_ApprovedArtistId",
                table: "CustomerNails",
                column: "ApprovedArtistId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerNails_NailArtists_ApprovedArtistId",
                table: "CustomerNails",
                column: "ApprovedArtistId",
                principalTable: "NailArtists",
                principalColumn: "NailArtistId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerNails_NailArtists_ApprovedArtistId",
                table: "CustomerNails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerNails_ApprovedArtistId",
                table: "CustomerNails");

            migrationBuilder.DropColumn(
                name: "ApprovedArtistId",
                table: "CustomerNails");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "CustomerNails");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CustomerNails",
                type: "text",
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);
        }
    }
}

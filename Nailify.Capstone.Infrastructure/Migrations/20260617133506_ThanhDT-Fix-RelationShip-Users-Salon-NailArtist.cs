using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTFixRelationShipUsersSalonNailArtist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NailArtists_Salons_SalonId",
                table: "NailArtists");

            migrationBuilder.DropForeignKey(
                name: "FK_Salons_Users_ManagerId",
                table: "Salons");

            migrationBuilder.DropIndex(
                name: "IX_Salons_ManagerId",
                table: "Salons");

            migrationBuilder.DropIndex(
                name: "IX_NailArtists_SalonId",
                table: "NailArtists");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Salons");

            migrationBuilder.DropColumn(
                name: "SalonId",
                table: "NailArtists");

            migrationBuilder.AddColumn<Guid>(
                name: "SalonId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SalonId",
                table: "Users",
                column: "SalonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Salons_SalonId",
                table: "Users",
                column: "SalonId",
                principalTable: "Salons",
                principalColumn: "SalonId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Salons_SalonId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SalonId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SalonId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Salons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalonId",
                table: "NailArtists",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Salons_ManagerId",
                table: "Salons",
                column: "ManagerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NailArtists_SalonId",
                table: "NailArtists",
                column: "SalonId");

            migrationBuilder.AddForeignKey(
                name: "FK_NailArtists_Salons_SalonId",
                table: "NailArtists",
                column: "SalonId",
                principalTable: "Salons",
                principalColumn: "SalonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Salons_Users_ManagerId",
                table: "Salons",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

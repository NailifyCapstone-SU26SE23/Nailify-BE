using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddNailArtistSalonRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalonId",
                table: "NailArtists",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_NailArtists_AccountId",
                table: "NailArtists",
                column: "AccountId",
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
                name: "FK_NailArtists_Users_AccountId",
                table: "NailArtists",
                column: "AccountId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NailArtists_Salons_SalonId",
                table: "NailArtists");

            migrationBuilder.DropForeignKey(
                name: "FK_NailArtists_Users_AccountId",
                table: "NailArtists");

            migrationBuilder.DropIndex(
                name: "IX_NailArtists_AccountId",
                table: "NailArtists");

            migrationBuilder.DropIndex(
                name: "IX_NailArtists_SalonId",
                table: "NailArtists");

            migrationBuilder.DropColumn(
                name: "SalonId",
                table: "NailArtists");
        }
    }
}

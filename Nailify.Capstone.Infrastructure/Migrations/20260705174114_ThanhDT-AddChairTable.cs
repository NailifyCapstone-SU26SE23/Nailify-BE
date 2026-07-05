using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddChairTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChairId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Chairs",
                columns: table => new
                {
                    ChairId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChairName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chairs", x => x.ChairId);
                    table.ForeignKey(
                        name: "FK_Chairs_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ChairId",
                table: "Bookings",
                column: "ChairId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingProcedures_AssignedArtistId",
                table: "BookingProcedures",
                column: "AssignedArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Chairs_SalonId",
                table: "Chairs",
                column: "SalonId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingProcedures_NailArtists_AssignedArtistId",
                table: "BookingProcedures",
                column: "AssignedArtistId",
                principalTable: "NailArtists",
                principalColumn: "NailArtistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Chairs_ChairId",
                table: "Bookings",
                column: "ChairId",
                principalTable: "Chairs",
                principalColumn: "ChairId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingProcedures_NailArtists_AssignedArtistId",
                table: "BookingProcedures");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Chairs_ChairId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Chairs");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ChairId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_BookingProcedures_AssignedArtistId",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "ChairId",
                table: "Bookings");
        }
    }
}

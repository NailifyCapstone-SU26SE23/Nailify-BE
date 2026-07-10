using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddSalonOffDayTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ProposedBookingDate",
                table: "Bookings",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposedBy",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ProposedStartTime",
                table: "Bookings",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RescheduleReason",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WarrantyForBookingId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SalonOffDates",
                columns: table => new
                {
                    SalonOffDateId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonOffDates", x => x.SalonOffDateId);
                    table.ForeignKey(
                        name: "FK_SalonOffDates_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_WarrantyForBookingId",
                table: "Bookings",
                column: "WarrantyForBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_SalonOffDates_SalonId",
                table: "SalonOffDates",
                column: "SalonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Bookings_WarrantyForBookingId",
                table: "Bookings",
                column: "WarrantyForBookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Bookings_WarrantyForBookingId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "SalonOffDates");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_WarrantyForBookingId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ProposedBookingDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ProposedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ProposedStartTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RescheduleReason",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "WarrantyForBookingId",
                table: "Bookings");
        }
    }
}

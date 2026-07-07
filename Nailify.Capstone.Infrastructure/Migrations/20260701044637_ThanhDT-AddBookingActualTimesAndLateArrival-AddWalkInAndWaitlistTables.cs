using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddBookingActualTimesAndLateArrivalAddWalkInAndWaitlistTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualCheckInTime",
                table: "Bookings",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartTime",
                table: "Bookings",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLateArrival",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BookingWaitlists",
                columns: table => new
                {
                    WailistId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreferredNailArtistId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequesetedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RequestedStartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EstimatedDuration = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NotifiedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ConvertedBookingId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingWaitlists", x => x.WailistId);
                    table.ForeignKey(
                        name: "FK_BookingWaitlists_Bookings_ConvertedBookingId",
                        column: x => x.ConvertedBookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BookingWaitlists_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingWaitlists_NailArtists_PreferredNailArtistId",
                        column: x => x.PreferredNailArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BookingWaitlists_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WalkInQueues",
                columns: table => new
                {
                    QueueId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalBookingId = table.Column<Guid>(type: "uuid", nullable: true),
                    GuestName = table.Column<string>(type: "text", nullable: true),
                    GuestPhone = table.Column<string>(type: "text", nullable: true),
                    QueuePosition = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CalledTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ServiceStartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AssignedNailArtistId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestNote = table.Column<string>(type: "text", nullable: true),
                    EstimatedWait = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalkInQueues", x => x.QueueId);
                    table.ForeignKey(
                        name: "FK_WalkInQueues_Bookings_OriginalBookingId",
                        column: x => x.OriginalBookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WalkInQueues_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WalkInQueues_NailArtists_AssignedNailArtistId",
                        column: x => x.AssignedNailArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WalkInQueues_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_ConvertedBookingId",
                table: "BookingWaitlists",
                column: "ConvertedBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_CustomerId",
                table: "BookingWaitlists",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_PreferredNailArtistId",
                table: "BookingWaitlists",
                column: "PreferredNailArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingWaitlists_SalonId",
                table: "BookingWaitlists",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_WalkInQueues_AssignedNailArtistId",
                table: "WalkInQueues",
                column: "AssignedNailArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_WalkInQueues_CustomerId",
                table: "WalkInQueues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_WalkInQueues_OriginalBookingId",
                table: "WalkInQueues",
                column: "OriginalBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_WalkInQueues_SalonId",
                table: "WalkInQueues",
                column: "SalonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingWaitlists");

            migrationBuilder.DropTable(
                name: "WalkInQueues");

            migrationBuilder.DropColumn(
                name: "ActualCheckInTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ActualStartTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsLateArrival",
                table: "Bookings");
        }
    }
}

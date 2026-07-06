using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddFieldToBookingProcedureProcedureBookWailist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveDuration",
                table: "Procedures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CanOverlap",
                table: "Procedures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PassiveDuration",
                table: "Procedures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActiveDuration",
                table: "BookingProcedures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndTime",
                table: "BookingProcedures",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartTime",
                table: "BookingProcedures",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedArtistId",
                table: "BookingProcedures",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanOverlap",
                table: "BookingProcedures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "BookingProcedures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedEndTime",
                table: "BookingProcedures",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EstimatedStartTime",
                table: "BookingProcedures",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassiveDuration",
                table: "BookingProcedures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WaitlistItems",
                columns: table => new
                {
                    WaitlistItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    WaitlistId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailVariantId = table.Column<int>(type: "integer", nullable: true),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerNailId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitlistItems", x => x.WaitlistItemId);
                    table.ForeignKey(
                        name: "FK_WaitlistItems_BookingWaitlists_WaitlistId",
                        column: x => x.WaitlistId,
                        principalTable: "BookingWaitlists",
                        principalColumn: "WailistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WaitlistItems_CustomerNails_CustomerNailId",
                        column: x => x.CustomerNailId,
                        principalTable: "CustomerNails",
                        principalColumn: "CustomerNailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaitlistItems_NailVariants_NailVariantId",
                        column: x => x.NailVariantId,
                        principalTable: "NailVariants",
                        principalColumn: "NailVariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaitlistItems_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistItems_CustomerNailId",
                table: "WaitlistItems",
                column: "CustomerNailId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistItems_NailVariantId",
                table: "WaitlistItems",
                column: "NailVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistItems_ServiceId",
                table: "WaitlistItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistItems_WaitlistId",
                table: "WaitlistItems",
                column: "WaitlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaitlistItems");

            migrationBuilder.DropColumn(
                name: "ActiveDuration",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "CanOverlap",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "PassiveDuration",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "ActiveDuration",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "ActualEndTime",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "ActualStartTime",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "AssignedArtistId",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "CanOverlap",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "EstimatedEndTime",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "EstimatedStartTime",
                table: "BookingProcedures");

            migrationBuilder.DropColumn(
                name: "PassiveDuration",
                table: "BookingProcedures");
        }
    }
}

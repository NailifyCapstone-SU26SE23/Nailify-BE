using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddStaffTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffTransfers",
                columns: table => new
                {
                    StaffTransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailArtistId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromSalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToSalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffTransfers", x => x.StaffTransferId);
                    table.ForeignKey(
                        name: "FK_StaffTransfers_NailArtists_NailArtistId",
                        column: x => x.NailArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffTransfers_Salons_FromSalonId",
                        column: x => x.FromSalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffTransfers_Salons_ToSalonId",
                        column: x => x.ToSalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffTransfers_FromSalonId",
                table: "StaffTransfers",
                column: "FromSalonId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffTransfers_NailArtistId_StartDate_EndDate",
                table: "StaffTransfers",
                columns: new[] { "NailArtistId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StaffTransfers_ToSalonId",
                table: "StaffTransfers",
                column: "ToSalonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffTransfers");
        }
    }
}

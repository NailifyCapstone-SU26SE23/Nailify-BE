using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddNailArtistBreakTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NailArtistBreaks",
                columns: table => new
                {
                    NailArtistBreakId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailArtistId = table.Column<Guid>(type: "uuid", nullable: false),
                    BreakDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailArtistBreaks", x => x.NailArtistBreakId);
                    table.ForeignKey(
                        name: "FK_NailArtistBreaks_NailArtists_NailArtistId",
                        column: x => x.NailArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NailArtistBreaks_NailArtistId",
                table: "NailArtistBreaks",
                column: "NailArtistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NailArtistBreaks");
        }
    }
}

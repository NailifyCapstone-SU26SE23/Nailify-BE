using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAddBookingRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingRatings",
                columns: table => new
                {
                    BookingRatingId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OverallScore = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ServiceQuality = table.Column<int>(type: "integer", nullable: true),
                    Punctuality = table.Column<int>(type: "integer", nullable: true),
                    Cleanliness = table.Column<int>(type: "integer", nullable: true),
                    IsUpdated = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRatings", x => x.BookingRatingId);
                    table.CheckConstraint("CK_BookingRating_Scores", "\"OverallScore\" BETWEEN 1 AND 5 AND (\"ServiceQuality\" IS NULL OR \"ServiceQuality\" BETWEEN 1 AND 5) AND (\"Punctuality\" IS NULL OR \"Punctuality\" BETWEEN 1 AND 5) AND (\"Cleanliness\" IS NULL OR \"Cleanliness\" BETWEEN 1 AND 5)");
                    table.ForeignKey(
                        name: "FK_BookingRatings_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingRatings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRatings_BookingId",
                table: "BookingRatings",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingRatings_CreatedAt",
                table: "BookingRatings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRatings_CustomerId",
                table: "BookingRatings",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingRatings");
        }
    }
}

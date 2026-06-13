using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTRecreateNailRequiredSkillTableNailVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "SkillTypes",
                columns: table => new
                {
                    SkillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTypes", x => x.SkillTypeId);
                });

            migrationBuilder.CreateTable(
                name: "NailRequiredSkills",
                columns: table => new
                {
                    NailRequiredSkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailVariantId = table.Column<int>(type: "integer", nullable: false),
                    SkillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailRequiredSkills", x => x.NailRequiredSkillId);
                    table.ForeignKey(
                        name: "FK_NailRequiredSkills_NailVariants_NailVariantId",
                        column: x => x.NailVariantId,
                        principalTable: "NailVariants",
                        principalColumn: "NailVariantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NailRequiredSkills_SkillTypes_SkillTypeId",
                        column: x => x.SkillTypeId,
                        principalTable: "SkillTypes",
                        principalColumn: "SkillTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailArtistId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<string>(type: "text", nullable: false),
                    TotalDuration = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckInImageUrl = table.Column<string>(type: "text", nullable: true),
                    CheckOutImagesUrl = table.Column<string>(type: "text", nullable: true),
                    QRCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_NailArtists_NailArtistId",
                        column: x => x.NailArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId");
                    table.ForeignKey(
                        name: "FK_Bookings_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NailArtistSkills",
                columns: table => new
                {
                    NailArtistSkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailArtistId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailArtistSkills", x => x.NailArtistSkillId);
                    table.ForeignKey(
                        name: "FK_NailArtistSkills_NailArtists_NailArtistId",
                        column: x => x.NailArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NailArtistSkills_SkillTypes_SkillTypeId",
                        column: x => x.SkillTypeId,
                        principalTable: "SkillTypes",
                        principalColumn: "SkillTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingHistories",
                columns: table => new
                {
                    BookingHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingHistories", x => x.BookingHistoryId);
                    table.ForeignKey(
                        name: "FK_BookingHistories_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingHistories_Users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "BookingItems",
                columns: table => new
                {
                    BookingItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    NailVariantId = table.Column<int>(type: "integer", nullable: true),
                    CustomerNailId = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItems", x => x.BookingItemId);
                    table.ForeignKey(
                        name: "FK_BookingItems_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingItems_CustomerNails_CustomerNailId",
                        column: x => x.CustomerNailId,
                        principalTable: "CustomerNails",
                        principalColumn: "CustomerNailId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BookingItems_NailVariants_NailVariantId",
                        column: x => x.NailVariantId,
                        principalTable: "NailVariants",
                        principalColumn: "NailVariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingItems_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_ActorId",
                table: "BookingHistories",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_BookingId",
                table: "BookingHistories",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_BookingId",
                table: "BookingItems",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_CustomerNailId",
                table: "BookingItems",
                column: "CustomerNailId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_NailVariantId",
                table: "BookingItems",
                column: "NailVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_ServiceId",
                table: "BookingItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerId",
                table: "Bookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_NailArtistId",
                table: "Bookings",
                column: "NailArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SalonId",
                table: "Bookings",
                column: "SalonId");
            
            migrationBuilder.CreateIndex(
                name: "IX_NailArtistSkills_NailArtistId_SkillTypeId",
                table: "NailArtistSkills",
                columns: new[] { "NailArtistId", "SkillTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NailArtistSkills_SkillTypeId",
                table: "NailArtistSkills",
                column: "SkillTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NailRequiredSkills_NailVariantId_SkillTypeId",
                table: "NailRequiredSkills",
                columns: new[] { "NailVariantId", "SkillTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NailRequiredSkills_SkillTypeId",
                table: "NailRequiredSkills",
                column: "SkillTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddCustomerNailRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerNails_NailArtists_ApprovedArtistId",
                table: "CustomerNails");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerNails_Salons_SalonId",
                table: "CustomerNails");

            migrationBuilder.DropIndex(
                name: "IX_CustomerNails_ApprovedArtistId",
                table: "CustomerNails");

            migrationBuilder.DropColumn(
                name: "ApprovedArtistId",
                table: "CustomerNails");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "CustomerNails");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CustomerNails",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateTable(
                name: "CustomerNailRequests",
                columns: table => new
                {
                    CustomerNailRequestId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CustomerNailId = table.Column<int>(type: "integer", nullable: false),
                    SalonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RejectReason = table.Column<string>(type: "text", nullable: true),
                    ApprovedArtistId = table.Column<Guid>(type: "uuid", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNailRequests", x => x.CustomerNailRequestId);
                    table.ForeignKey(
                        name: "FK_CustomerNailRequests_CustomerNails_CustomerNailId",
                        column: x => x.CustomerNailId,
                        principalTable: "CustomerNails",
                        principalColumn: "CustomerNailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerNailRequests_NailArtists_ApprovedArtistId",
                        column: x => x.ApprovedArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerNailRequests_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "SalonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNailRequests_ApprovedArtistId",
                table: "CustomerNailRequests",
                column: "ApprovedArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNailRequests_CustomerNailId",
                table: "CustomerNailRequests",
                column: "CustomerNailId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNailRequests_SalonId",
                table: "CustomerNailRequests",
                column: "SalonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerNails_Salons_SalonId",
                table: "CustomerNails",
                column: "SalonId",
                principalTable: "Salons",
                principalColumn: "SalonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerNails_Salons_SalonId",
                table: "CustomerNails");

            migrationBuilder.DropTable(
                name: "CustomerNailRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CustomerNails",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Active");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedArtistId",
                table: "CustomerNails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "CustomerNails",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_ApprovedArtistId",
                table: "CustomerNails",
                column: "ApprovedArtistId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerNails_NailArtists_ApprovedArtistId",
                table: "CustomerNails",
                column: "ApprovedArtistId",
                principalTable: "NailArtists",
                principalColumn: "NailArtistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerNails_Salons_SalonId",
                table: "CustomerNails",
                column: "SalonId",
                principalTable: "Salons",
                principalColumn: "SalonId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

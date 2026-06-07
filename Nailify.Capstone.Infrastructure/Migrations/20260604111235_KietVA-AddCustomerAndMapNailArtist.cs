using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class KietVAAddCustomerAndMapNailArtist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoyaltyPoint = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SkinTone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    Occupation = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false, defaultValue: ""),
                    NailCondition = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, defaultValue: ""),
                    PersonaId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}

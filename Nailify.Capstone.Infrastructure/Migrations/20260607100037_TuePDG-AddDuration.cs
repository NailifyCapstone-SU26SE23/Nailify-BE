using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAddDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "NailSurfaces",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "NailShapes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Components",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "NailSurfaces");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "NailShapes");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Components");
        }
    }
}

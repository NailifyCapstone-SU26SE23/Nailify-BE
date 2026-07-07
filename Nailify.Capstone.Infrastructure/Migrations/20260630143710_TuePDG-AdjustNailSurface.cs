using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAdjustNailSurface : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FinishType",
                table: "NailSurfaces",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "HueOffset",
                table: "NailSurfaces",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "LightnessOffset",
                table: "NailSurfaces",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "SaturationOffset",
                table: "NailSurfaces",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinishType",
                table: "NailSurfaces");

            migrationBuilder.DropColumn(
                name: "HueOffset",
                table: "NailSurfaces");

            migrationBuilder.DropColumn(
                name: "LightnessOffset",
                table: "NailSurfaces");

            migrationBuilder.DropColumn(
                name: "SaturationOffset",
                table: "NailSurfaces");
        }
    }
}

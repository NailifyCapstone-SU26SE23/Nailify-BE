using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGRemoveSkillInVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "NailVariants");

            migrationBuilder.DropColumn(
                name: "Form",
                table: "NailVariants");

            migrationBuilder.DropColumn(
                name: "Material",
                table: "NailVariants");

            migrationBuilder.DropColumn(
                name: "Precision",
                table: "NailVariants");

            migrationBuilder.DropColumn(
                name: "Speed",
                table: "NailVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "NailVariants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Form",
                table: "NailVariants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "NailVariants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Precision",
                table: "NailVariants",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Speed",
                table: "NailVariants",
                type: "integer",
                nullable: true);
        }
    }
}

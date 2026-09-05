using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAddSalonConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonaId",
                table: "Customers");

            migrationBuilder.AddColumn<decimal>(
                name: "DepositConfig",
                table: "Salons",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositConfig",
                table: "Salons");

            migrationBuilder.AddColumn<string>(
                name: "PersonaId",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "");
        }
    }
}

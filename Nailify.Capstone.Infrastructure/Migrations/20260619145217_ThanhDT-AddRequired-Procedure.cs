using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddRequiredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "Procedures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "BookingProcedures",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "BookingProcedures");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddCustomerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HandShape",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkinShade",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandShape",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SkinShade",
                table: "Customers");
        }
    }
}

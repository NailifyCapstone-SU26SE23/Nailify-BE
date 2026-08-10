using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTSupportCustomNailProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProcedureId",
                table: "NailProcedures",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinutes",
                table: "NailProcedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomStep",
                table: "NailProcedures",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "NailProcedures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "NailProcedures",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "NailProcedures",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedMinutes",
                table: "NailProcedures");

            migrationBuilder.DropColumn(
                name: "IsCustomStep",
                table: "NailProcedures");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "NailProcedures");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "NailProcedures");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "NailProcedures");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProcedureId",
                table: "NailProcedures",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}

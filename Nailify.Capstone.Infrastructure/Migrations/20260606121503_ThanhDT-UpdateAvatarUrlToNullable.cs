using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTUpdateAvatarUrlToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Salons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SkinTone",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PersonaId",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Occupation",
                table: "Customers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NailCondition",
                table: "Customers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldDefaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Salons_ManagerId",
                table: "Salons",
                column: "ManagerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Salons_Users_ManagerId",
                table: "Salons",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salons_Users_ManagerId",
                table: "Salons");

            migrationBuilder.DropIndex(
                name: "IX_Salons_ManagerId",
                table: "Salons");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Salons");

            migrationBuilder.AlterColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SkinTone",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PersonaId",
                table: "Customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Occupation",
                table: "Customers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NailCondition",
                table: "Customers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValue: "");
        }
    }
}

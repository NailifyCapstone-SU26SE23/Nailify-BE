using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGFixPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "ReceivedCount",
                table: "UserPromotionUsages",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Situation",
                table: "Promotions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ReceivedCount",
                table: "UserPromotionUsages");

            migrationBuilder.DropColumn(
                name: "Situation",
                table: "Promotions");

        }
    }
}

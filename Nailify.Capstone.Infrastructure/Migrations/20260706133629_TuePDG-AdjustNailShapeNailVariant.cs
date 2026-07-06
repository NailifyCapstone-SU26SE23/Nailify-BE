using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAdjustNailShapeNailVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NailVariants_NailDesigns_NailDesignId",
                table: "NailVariants");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "NailShapes");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "NailShapes");

            migrationBuilder.AlterColumn<int>(
                name: "NailDesignId",
                table: "NailVariants",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "NailVariantId",
                table: "NailProcedures",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "CustomerNailId",
                table: "NailProcedures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShapeMethodConfigId",
                table: "BookingItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShapeMethodConfigs",
                columns: table => new
                {
                    ShapeMethodConfigId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NailShapeId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeMethodConfigs", x => x.ShapeMethodConfigId);
                    table.ForeignKey(
                        name: "FK_ShapeMethodConfigs_NailShapes_NailShapeId",
                        column: x => x.NailShapeId,
                        principalTable: "NailShapes",
                        principalColumn: "NailShapeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NailProcedures_CustomerNailId",
                table: "NailProcedures",
                column: "CustomerNailId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_ShapeMethodConfigId",
                table: "BookingItems",
                column: "ShapeMethodConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ShapeMethodConfigs_NailShapeId",
                table: "ShapeMethodConfigs",
                column: "NailShapeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_ShapeMethodConfigs_ShapeMethodConfigId",
                table: "BookingItems",
                column: "ShapeMethodConfigId",
                principalTable: "ShapeMethodConfigs",
                principalColumn: "ShapeMethodConfigId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NailProcedures_CustomerNails_CustomerNailId",
                table: "NailProcedures",
                column: "CustomerNailId",
                principalTable: "CustomerNails",
                principalColumn: "CustomerNailId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NailVariants_NailDesigns_NailDesignId",
                table: "NailVariants",
                column: "NailDesignId",
                principalTable: "NailDesigns",
                principalColumn: "NailDesignId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_ShapeMethodConfigs_ShapeMethodConfigId",
                table: "BookingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_NailProcedures_CustomerNails_CustomerNailId",
                table: "NailProcedures");

            migrationBuilder.DropForeignKey(
                name: "FK_NailVariants_NailDesigns_NailDesignId",
                table: "NailVariants");

            migrationBuilder.DropTable(
                name: "ShapeMethodConfigs");

            migrationBuilder.DropIndex(
                name: "IX_NailProcedures_CustomerNailId",
                table: "NailProcedures");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_ShapeMethodConfigId",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "CustomerNailId",
                table: "NailProcedures");

            migrationBuilder.DropColumn(
                name: "ShapeMethodConfigId",
                table: "BookingItems");

            migrationBuilder.AlterColumn<int>(
                name: "NailDesignId",
                table: "NailVariants",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "NailShapes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "NailShapes",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "NailVariantId",
                table: "NailProcedures",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NailVariants_NailDesigns_NailDesignId",
                table: "NailVariants",
                column: "NailDesignId",
                principalTable: "NailDesigns",
                principalColumn: "NailDesignId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

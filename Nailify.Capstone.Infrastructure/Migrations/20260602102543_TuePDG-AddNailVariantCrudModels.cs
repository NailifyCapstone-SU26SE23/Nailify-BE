using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAddNailVariantCrudModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    ComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    ComponentType = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.ComponentId);
                });

            migrationBuilder.CreateTable(
                name: "NailShapes",
                columns: table => new
                {
                    NailShapeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailShapes", x => x.NailShapeId);
                });

            migrationBuilder.CreateTable(
                name: "NailSurfaces",
                columns: table => new
                {
                    NailSurfaceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShaderParam = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailSurfaces", x => x.NailSurfaceId);
                });

            migrationBuilder.CreateTable(
                name: "NailVariants",
                columns: table => new
                {
                    NailVariantId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NailShapeId = table.Column<int>(type: "integer", nullable: false),
                    NailSurfaceId = table.Column<int>(type: "integer", nullable: false),
                    NailDesignId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    Precision = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Form = table.Column<string>(type: "text", nullable: false),
                    Material = table.Column<string>(type: "text", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailVariants", x => x.NailVariantId);
                    table.ForeignKey(
                        name: "FK_NailVariants_NailDesigns_NailDesignId",
                        column: x => x.NailDesignId,
                        principalTable: "NailDesigns",
                        principalColumn: "NailDesignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NailVariants_NailShapes_NailShapeId",
                        column: x => x.NailShapeId,
                        principalTable: "NailShapes",
                        principalColumn: "NailShapeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NailVariants_NailSurfaces_NailSurfaceId",
                        column: x => x.NailSurfaceId,
                        principalTable: "NailSurfaces",
                        principalColumn: "NailSurfaceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NailComponents",
                columns: table => new
                {
                    NailComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentId = table.Column<int>(type: "integer", nullable: false),
                    NailVariantId = table.Column<int>(type: "integer", nullable: false),
                    PosX = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PosY = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FingerIndex = table.Column<int>(type: "integer", nullable: false, comment: "-1 = whole hand, 0-9 = specific finger index"),
                    ConfigJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailComponents", x => x.NailComponentId);
                    table.ForeignKey(
                        name: "FK_NailComponents_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NailComponents_NailVariants_NailVariantId",
                        column: x => x.NailVariantId,
                        principalTable: "NailVariants",
                        principalColumn: "NailVariantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NailComponents_ComponentId",
                table: "NailComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_NailComponents_NailVariantId",
                table: "NailComponents",
                column: "NailVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_NailVariants_NailDesignId",
                table: "NailVariants",
                column: "NailDesignId");

            migrationBuilder.CreateIndex(
                name: "IX_NailVariants_NailShapeId",
                table: "NailVariants",
                column: "NailShapeId");

            migrationBuilder.CreateIndex(
                name: "IX_NailVariants_NailSurfaceId",
                table: "NailVariants",
                column: "NailSurfaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NailComponents");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "NailVariants");

            migrationBuilder.DropTable(
                name: "NailShapes");

            migrationBuilder.DropTable(
                name: "NailSurfaces");
        }
    }
}

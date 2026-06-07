using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAddCustomerNailCustomizationCrud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerComponents",
                columns: table => new
                {
                    CustomerComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    ComponentType = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CustomDataJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerComponents", x => x.CustomerComponentId);
                    table.ForeignKey(
                        name: "FK_CustomerComponents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerNails",
                columns: table => new
                {
                    CustomerNailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    NailShapeId = table.Column<int>(type: "integer", nullable: false),
                    NailSurfaceId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomColor = table.Column<string>(type: "text", nullable: false),
                    CustomMaterial = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    BasedOnNailVariantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNails", x => x.CustomerNailId);
                    table.ForeignKey(
                        name: "FK_CustomerNails_NailShapes_NailShapeId",
                        column: x => x.NailShapeId,
                        principalTable: "NailShapes",
                        principalColumn: "NailShapeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerNails_NailSurfaces_NailSurfaceId",
                        column: x => x.NailSurfaceId,
                        principalTable: "NailSurfaces",
                        principalColumn: "NailSurfaceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerNails_NailVariants_BasedOnNailVariantId",
                        column: x => x.BasedOnNailVariantId,
                        principalTable: "NailVariants",
                        principalColumn: "NailVariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerNails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerNailComponents",
                columns: table => new
                {
                    CustomerNailComponentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerNailId = table.Column<int>(type: "integer", nullable: false),
                    ComponentId = table.Column<int>(type: "integer", nullable: true),
                    CustomerComponentId = table.Column<int>(type: "integer", nullable: true),
                    PosX = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PosY = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FingerIndex = table.Column<int>(type: "integer", nullable: false, comment: "-1 = whole hand, 0-9 = specific finger index"),
                    ConfigJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNailComponents", x => x.CustomerNailComponentId);
                    table.CheckConstraint("CK_CustomerNailComponent_OneComponent", "(\"ComponentId\" IS NOT NULL AND \"CustomerComponentId\" IS NULL) OR (\"ComponentId\" IS NULL AND \"CustomerComponentId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_CustomerNailComponents_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerNailComponents_CustomerComponents_CustomerComponent~",
                        column: x => x.CustomerComponentId,
                        principalTable: "CustomerComponents",
                        principalColumn: "CustomerComponentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerNailComponents_CustomerNails_CustomerNailId",
                        column: x => x.CustomerNailId,
                        principalTable: "CustomerNails",
                        principalColumn: "CustomerNailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerComponents_UserId",
                table: "CustomerComponents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNailComponents_ComponentId",
                table: "CustomerNailComponents",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNailComponents_CustomerComponentId",
                table: "CustomerNailComponents",
                column: "CustomerComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNailComponents_CustomerNailId",
                table: "CustomerNailComponents",
                column: "CustomerNailId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_BasedOnNailVariantId",
                table: "CustomerNails",
                column: "BasedOnNailVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_NailShapeId",
                table: "CustomerNails",
                column: "NailShapeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_NailSurfaceId",
                table: "CustomerNails",
                column: "NailSurfaceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNails_UserId",
                table: "CustomerNails",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerNailComponents");

            migrationBuilder.DropTable(
                name: "CustomerComponents");

            migrationBuilder.DropTable(
                name: "CustomerNails");
        }
    }
}

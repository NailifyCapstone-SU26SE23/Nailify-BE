using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddSkillTypeNailArtistSkillNailRequiredSkillTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SkillTypes",
                columns: table => new
                {
                    SkillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillTypes", x => x.SkillTypeId);
                });

            migrationBuilder.CreateTable(
                name: "NailArtistSkills",
                columns: table => new
                {
                    NailArtistSkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailArtistId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailArtistSkills", x => x.NailArtistSkillId);
                    table.ForeignKey(
                        name: "FK_NailArtistSkills_NailArtists_NailArtistId",
                        column: x => x.NailArtistId,
                        principalTable: "NailArtists",
                        principalColumn: "NailArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NailArtistSkills_SkillTypes_SkillTypeId",
                        column: x => x.SkillTypeId,
                        principalTable: "SkillTypes",
                        principalColumn: "SkillTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NailRequiredSkills",
                columns: table => new
                {
                    NailRequiredSkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailDesignId = table.Column<int>(type: "integer", nullable: false),
                    SkillTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NailRequiredSkills", x => x.NailRequiredSkillId);
                    table.ForeignKey(
                        name: "FK_NailRequiredSkills_NailDesigns_NailDesignId",
                        column: x => x.NailDesignId,
                        principalTable: "NailDesigns",
                        principalColumn: "NailDesignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NailRequiredSkills_SkillTypes_SkillTypeId",
                        column: x => x.SkillTypeId,
                        principalTable: "SkillTypes",
                        principalColumn: "SkillTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NailArtistSkills_NailArtistId_SkillTypeId",
                table: "NailArtistSkills",
                columns: new[] { "NailArtistId", "SkillTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NailArtistSkills_SkillTypeId",
                table: "NailArtistSkills",
                column: "SkillTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NailRequiredSkills_NailDesignId_SkillTypeId",
                table: "NailRequiredSkills",
                columns: new[] { "NailDesignId", "SkillTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NailRequiredSkills_SkillTypeId",
                table: "NailRequiredSkills",
                column: "SkillTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NailArtistSkills");

            migrationBuilder.DropTable(
                name: "NailRequiredSkills");

            migrationBuilder.DropTable(
                name: "SkillTypes");
        }
    }
}

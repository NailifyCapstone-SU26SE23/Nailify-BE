using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ThanhDTAddQuizQuizOptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredColorsJson",
                table: "Customers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredComplexity",
                table: "Customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PreferredNailShapeId",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredOccasionsJson",
                table: "Customers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredStylesJson",
                table: "Customers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    QuizQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.QuizQuestionId);
                });

            migrationBuilder.CreateTable(
                name: "QuizOptions",
                columns: table => new
                {
                    QuizOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizOptions", x => x.QuizOptionId);
                    table.ForeignKey(
                        name: "FK_QuizOptions_QuizQuestions_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestions",
                        principalColumn: "QuizQuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerQuizAnswers",
                columns: table => new
                {
                    CustomerQuizAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizOptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerQuizAnswers", x => x.CustomerQuizAnswerId);
                    table.ForeignKey(
                        name: "FK_CustomerQuizAnswers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerQuizAnswers_QuizOptions_QuizOptionId",
                        column: x => x.QuizOptionId,
                        principalTable: "QuizOptions",
                        principalColumn: "QuizOptionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerQuizAnswers_QuizQuestions_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestions",
                        principalColumn: "QuizQuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerQuizAnswers_CustomerId",
                table: "CustomerQuizAnswers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerQuizAnswers_QuizOptionId",
                table: "CustomerQuizAnswers",
                column: "QuizOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerQuizAnswers_QuizQuestionId",
                table: "CustomerQuizAnswers",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOptions_QuizQuestionId",
                table: "QuizOptions",
                column: "QuizQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerQuizAnswers");

            migrationBuilder.DropTable(
                name: "QuizOptions");

            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropColumn(
                name: "PreferredColorsJson",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PreferredComplexity",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PreferredNailShapeId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PreferredOccasionsJson",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PreferredStylesJson",
                table: "Customers");
        }
    }
}

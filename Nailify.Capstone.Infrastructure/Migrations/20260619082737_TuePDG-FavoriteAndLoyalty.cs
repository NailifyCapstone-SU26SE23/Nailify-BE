using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGFavoriteAndLoyalty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.AddColumn<int>(
                name: "LifetimePoints",
                table: "Customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoyaltyTierId",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CustomerNails",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateTable(
                name: "FavoriteNails",
                columns: table => new
                {
                    FavoriteNailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NailDesignId = table.Column<int>(type: "integer", nullable: true),
                    NailVariantId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteNails", x => x.FavoriteNailId);
                    table.CheckConstraint("CK_FavoriteNail_DesignOrVariant", "\"NailDesignId\" IS NOT NULL OR \"NailVariantId\" IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_FavoriteNails_NailDesigns_NailDesignId",
                        column: x => x.NailDesignId,
                        principalTable: "NailDesigns",
                        principalColumn: "NailDesignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteNails_NailVariants_NailVariantId",
                        column: x => x.NailVariantId,
                        principalTable: "NailVariants",
                        principalColumn: "NailVariantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteNails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyTiers",
                columns: table => new
                {
                    LoyaltyTierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MinLifetimePoints = table.Column<int>(type: "integer", nullable: true),
                    MaxLifetimePoints = table.Column<int>(type: "integer", nullable: true),
                    DiscountRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    BackgroundColor = table.Column<string>(type: "text", nullable: true),
                    TextColor = table.Column<string>(type: "text", nullable: true),
                    ColorJson = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Active"),
                    SortOrder = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyTiers", x => x.LoyaltyTierId);
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyTransactions",
                columns: table => new
                {
                    LoyaltyTransactionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false, defaultValue: "Earned"),
                    LoyaltyTierIdAtTime = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoyaltyTransactions", x => x.LoyaltyTransactionId);
                    table.ForeignKey(
                        name: "FK_LoyaltyTransactions_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoyaltyTransactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoyaltyTransactions_LoyaltyTiers_LoyaltyTierIdAtTime",
                        column: x => x.LoyaltyTierIdAtTime,
                        principalTable: "LoyaltyTiers",
                        principalColumn: "LoyaltyTierId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_LoyaltyTierId",
                table: "Customers",
                column: "LoyaltyTierId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteNails_NailDesignId",
                table: "FavoriteNails",
                column: "NailDesignId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteNails_NailVariantId",
                table: "FavoriteNails",
                column: "NailVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteNails_UserId_NailDesignId_NailVariantId",
                table: "FavoriteNails",
                columns: new[] { "UserId", "NailDesignId", "NailVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTiers_MinLifetimePoints",
                table: "LoyaltyTiers",
                column: "MinLifetimePoints",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_BookingId",
                table: "LoyaltyTransactions",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_CustomerId",
                table: "LoyaltyTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransactions_LoyaltyTierIdAtTime",
                table: "LoyaltyTransactions",
                column: "LoyaltyTierIdAtTime");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_LoyaltyTiers_LoyaltyTierId",
                table: "Customers",
                column: "LoyaltyTierId",
                principalTable: "LoyaltyTiers",
                principalColumn: "LoyaltyTierId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_LoyaltyTiers_LoyaltyTierId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "FavoriteNails");

            migrationBuilder.DropTable(
                name: "LoyaltyTransactions");

            migrationBuilder.DropTable(
                name: "LoyaltyTiers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_LoyaltyTierId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "LifetimePoints",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LoyaltyTierId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "BookingProcedures");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "CustomerNails",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CustomerNails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGDesignPriceRangeVariantComputed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "NailDesigns",
                newName: "MinPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                table: "NailDesigns",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE "NailVariants" AS nv
                SET "Price" = price_calculations."CalculatedPrice"
                FROM (
                    SELECT nv2."NailVariantId",
                           COALESCE(ns."Price", 0)
                           + COALESCE(nsf."Price", 0)
                           + COALESCE(component_totals."TotalComponentPrice", 0) AS "CalculatedPrice"
                    FROM "NailVariants" AS nv2
                    INNER JOIN "NailShapes" AS ns ON ns."NailShapeId" = nv2."NailShapeId"
                    INNER JOIN "NailSurfaces" AS nsf ON nsf."NailSurfaceId" = nv2."NailSurfaceId"
                    LEFT JOIN (
                        SELECT nc."NailVariantId", SUM(c."Price") AS "TotalComponentPrice"
                        FROM "NailComponents" AS nc
                        INNER JOIN "Components" AS c ON c."ComponentId" = nc."ComponentId"
                        GROUP BY nc."NailVariantId"
                    ) AS component_totals ON component_totals."NailVariantId" = nv2."NailVariantId"
                ) AS price_calculations
                WHERE price_calculations."NailVariantId" = nv."NailVariantId";
                """);

            migrationBuilder.Sql("""
                UPDATE "NailDesigns" AS nd
                SET "MinPrice" = COALESCE(price_ranges."MinPrice", 0),
                    "MaxPrice" = COALESCE(price_ranges."MaxPrice", 0)
                FROM "NailDesigns" AS nd_source
                LEFT JOIN (
                    SELECT "NailDesignId", MIN("Price") AS "MinPrice", MAX("Price") AS "MaxPrice"
                    FROM "NailVariants"
                    GROUP BY "NailDesignId"
                ) AS price_ranges ON price_ranges."NailDesignId" = nd_source."NailDesignId"
                WHERE nd_source."NailDesignId" = nd."NailDesignId";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "NailDesigns");

            migrationBuilder.RenameColumn(
                name: "MinPrice",
                table: "NailDesigns",
                newName: "Price");
        }
    }
}

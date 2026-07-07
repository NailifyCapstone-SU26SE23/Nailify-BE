using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TuePDGAddDiscountInBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.Sql(
                """
                ALTER TABLE "Bookings"
                ALTER COLUMN "Price" TYPE numeric
                USING (
                    CASE
                        WHEN "Price" IS NULL OR btrim("Price") = '' THEN NULL
                        ELSE NULLIF(regexp_replace("Price", '[^0-9.-]', '', 'g'), '')::numeric
                    END
                );

                ALTER TABLE "Bookings"
                ALTER COLUMN "Price" DROP NOT NULL;
                """);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Bookings",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Bookings");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
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

            migrationBuilder.Sql(
                """
                UPDATE "Bookings"
                SET "Price" = 0
                WHERE "Price" IS NULL;

                ALTER TABLE "Bookings"
                ALTER COLUMN "Price" TYPE text
                USING "Price"::text;

                ALTER TABLE "Bookings"
                ALTER COLUMN "Price" SET NOT NULL;
                """);
        }
    }
}

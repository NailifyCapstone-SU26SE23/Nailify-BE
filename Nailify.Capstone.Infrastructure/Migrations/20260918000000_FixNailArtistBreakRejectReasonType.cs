using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Nailify.Capstone.Infrastructure.DBContext;

#nullable disable

namespace Nailify.Capstone.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(NailifyDbContext))]
    [Migration("20260918000000_FixNailArtistBreakRejectReasonType")]
    public partial class FixNailArtistBreakRejectReasonType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'NailArtistBreaks'
                          AND column_name = 'RejectReason'
                          AND data_type = 'ARRAY'
                    ) THEN
                        ALTER TABLE "NailArtistBreaks"
                        ALTER COLUMN "RejectReason" TYPE text
                        USING
                            CASE
                                WHEN "RejectReason" IS NULL THEN NULL
                                ELSE array_to_string("RejectReason", ', ')
                            END;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "NailArtistBreaks"
                ALTER COLUMN "RejectReason" TYPE character varying[]
                USING
                    CASE
                        WHEN "RejectReason" IS NULL OR "RejectReason" = '' THEN NULL
                        ELSE ARRAY["RejectReason"]
                    END;
                """);
        }
    }
}

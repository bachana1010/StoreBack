using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBarcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getBarcode]
            @barcodetext NVARCHAR(MAX)

            AS
            BEGIN
            SET NOCOUNT ON;
            SELECT * 
            FROM Barcodes WHERE Barcode = @barcodetext
            END";

        migrationBuilder.Sql(sp);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE getBarcode");

        }
    }
}

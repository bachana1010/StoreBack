using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBarcode2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getBarcode]
                @barcodetext NVARCHAR(MAX)

                 AS
                BEGIN
                SET NOCOUNT ON;
                SELECT TOP 1
                    Barcodes.Barcode, 
                    Barcodes.Name, 
                    Barcodes.Price, 
                    Barcodes.Unit, 
                    Barcodes.BranchId, 
                    Barcodes.OrganizationId,
                    Goodsin.Quantity

                FROM Barcodes
                INNER JOIN Goodsin ON Barcodes.Id = Goodsin.BarcodeId
                WHERE Barcodes.Barcode = @barcodetext
                ORDER BY Goodsin.EntryDate DESC 
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

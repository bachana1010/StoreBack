using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBarcode6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getBarcode]
                @barcodetext NVARCHAR(MAX),
                @branchId   INT

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
                WHERE Barcodes.Barcode = @barcodetext and Barcodes.BranchId = @branchId
                ORDER BY Goodsin.Id DESC 
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

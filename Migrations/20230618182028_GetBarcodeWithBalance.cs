using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBarcodeWithBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             var sp = @"
            CREATE OR ALTER PROCEDURE GetBarcodeWithBalance
                @barcodetext NVARCHAR(MAX),
                @branchId INT

            AS
            BEGIN
                SET NOCOUNT ON;

                SELECT 
                    Barcodes.Barcode, 
                    Barcodes.Name, 
                    Barcodes.Price, 
                    Barcodes.Unit, 
                    Barcodes.BranchId, 
                    Barcodes.OrganizationId,
                    ISNULL((SELECT SUM(Goodsin.Quantity) FROM Goodsin WHERE Goodsin.BarcodeId = Barcodes.Id), 0) - 
                    ISNULL((SELECT SUM(GoodsOut.Quantity) FROM GoodsOut WHERE GoodsOut.BarcodeId = Barcodes.Id), 0) AS Quantity
                FROM 
                    Barcodes
                WHERE 
                    Barcodes.Barcode = @barcodetext AND
                    Barcodes.BranchId = @branchId

            END";
            
            migrationBuilder.Sql(sp);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE GetBarcodeWithBalance");


        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AlterGetBarcodeWithBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
var createProcSql = @"
                CREATE OR ALTER PROCEDURE GetBarcodeWithBalance
                    @OrganizationId int = NULL,
                    @BranchId INT = NULL
                AS
                BEGIN
                SELECT 
                    Barcodes.Id,
                    Barcodes.Barcode, 
                    Barcodes.Name, 
                    Barcodes.Price, 
                    Barcodes.Unit, 
                    Barcodes.BranchId, 
                    Barcodes.OrganizationId,
                    Branches.BrancheName as BranchName,
                 CAST(ISNULL((SELECT SUM(Goodsin.Quantity) FROM Goodsin WHERE Goodsin.BarcodeId = Barcodes.Id), 0) - 
                        ISNULL((SELECT SUM(GoodsOut.Quantity) FROM GoodsOut WHERE GoodsOut.BarcodeId = Barcodes.Id), 0) AS REAL) AS Quantity
                FROM 
                    Barcodes
                FULL OUTER JOIN Branches ON Barcodes.BranchId = Branches.Id 
                WHERE 
                    (@OrganizationId IS NULL OR Barcodes.OrganizationId = @OrganizationId)
                    AND (@BranchId IS NULL OR Barcodes.BranchId = @BranchId)
                END
            ";

            migrationBuilder.Sql(createProcSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           var dropProcSql = "DROP PROCEDURE GetUserByEmail";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

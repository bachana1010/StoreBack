using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    public partial class GetManagerBalance : Migration
    {
         protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        
                CREATE PROCEDURE dbo.GetManagerBalance
                    @organizationId INT,
                    @branchId INT = NULL
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
                        Branches.BrancheName AS BranchName,
                        CAST(ISNULL((SELECT SUM(Goodsin.Quantity) FROM Goodsin WHERE Goodsin.BarcodeId = Barcodes.Id), 0) - 
                            ISNULL((SELECT SUM(GoodsOut.Quantity) FROM GoodsOut WHERE GoodsOut.BarcodeId = Barcodes.Id), 0) AS REAL) AS Quantity
                    FROM 
                        Barcodes
                    INNER JOIN
                        Branches ON Barcodes.BranchId = Branches.Id
                    WHERE 
                        Barcodes.OrganizationId = @organizationId
                    AND (@branchId IS NULL OR Barcodes.BranchId = @branchId)
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql("DROP PROCEDURE GetManagerBalance");

        }
    }
}

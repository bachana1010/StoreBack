using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetManagerBalancePagination1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                   migrationBuilder.Sql(@"
        
                CREATE OR ALTER PROCEDURE dbo.GetManagerBalance
                    @organizationId INT,
                    @branchId INT = NULL,
                    @pageNumber INT = 1,
                    @pageSize INT = 5,
                    @totalCount INT OUTPUT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    -- Calculate the starting offset based on the page size and number
                    DECLARE @startOffset INT = (@pageNumber - 1) * @pageSize;

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
                    ORDER BY
                        Barcodes.Id
                    OFFSET @startOffset ROWS
                    FETCH NEXT @pageSize ROWS ONLY;

                    -- Get total count for pagination
                    SELECT @totalCount = COUNT(*)
                    FROM 
                        Barcodes
                    WHERE 
                        Barcodes.OrganizationId = @organizationId
                    AND (@branchId IS NULL OR Barcodes.BranchId = @branchId);
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                                    migrationBuilder.Sql("DROP PROCEDURE GetManagerBalance");

        }
    }
}

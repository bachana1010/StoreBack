using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AlterGetBarcodeWithBalanceAddPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {   
            var createProcSql = @"
                CREATE OR ALTER PROCEDURE GetBarcodeWithBalance
                    @OrganizationId int = NULL,
                    @BranchId INT = NULL,
                    @pageNumber INT = 1,
                    @pageSize INT = 5
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Calculate the starting offset based on the page size and number
                    DECLARE @startOffset INT = (@pageNumber - 1) * @pageSize;

                    -- Calculate total count
                    DECLARE @totalCount INT;
                    SELECT @totalCount = COUNT(*)
                    FROM 
                        Barcodes
                    WHERE 
                        (@OrganizationId IS NULL OR Barcodes.OrganizationId = @OrganizationId)
                    AND (@BranchId IS NULL OR Barcodes.BranchId = @BranchId);

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
                            ISNULL((SELECT SUM(GoodsOut.Quantity) FROM GoodsOut WHERE GoodsOut.BarcodeId = Barcodes.Id), 0) AS REAL) AS Quantity,
                        @totalCount as TotalCount
                    FROM 
                        Barcodes
                    FULL OUTER JOIN Branches ON Barcodes.BranchId = Branches.Id 
                    WHERE 
                        (@OrganizationId IS NULL OR Barcodes.OrganizationId = @OrganizationId)
                        AND (@BranchId IS NULL OR Barcodes.BranchId = @BranchId)
                    ORDER BY
                        Barcodes.Id
                    OFFSET @startOffset ROWS
                    FETCH NEXT @pageSize ROWS ONLY;
                END
            ";

            migrationBuilder.Sql(createProcSql);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            var dropProcSql = "DROP PROCEDURE GetBarcodeWithBalance";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

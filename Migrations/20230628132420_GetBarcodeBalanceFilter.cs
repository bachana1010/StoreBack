using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBarcodeBalanceFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                CREATE OR ALTER PROCEDURE GetBarcodeWithBalance
                    @OrganizationId int = NULL,
                    @BranchId INT = NULL,
                    @Name nvarchar(255) = NULL,
                    @PriceOperator nvarchar(1) = NULL,
                    @PriceValue decimal(18,2) = NULL,
                    @QuantityOperator nvarchar(1) = NULL,
                    @QuantityValue decimal(18,2) = NULL,
                    @pageNumber INT = 1,
                    @pageSize INT = 5
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Calculate the starting offset based on the page size and number
                    DECLARE @startOffset INT = (@pageNumber - 1) * @pageSize;

                    DECLARE @SQL nvarchar(4000) = N'
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
                        COUNT(*) OVER() as TotalCount
                    FROM 
                        Barcodes
                    FULL OUTER JOIN Branches ON Barcodes.BranchId = Branches.Id 
                    WHERE 
                        (@OrganizationId IS NULL OR Barcodes.OrganizationId = @OrganizationId)
                        AND (@BranchId IS NULL OR Barcodes.BranchId = @BranchId)';

                    IF @Name IS NOT NULL
                    BEGIN
                        SET @SQL += ' AND Barcodes.Name LIKE ''%' + @Name + '%''';
                    END

                    IF @PriceOperator IS NOT NULL AND @PriceValue IS NOT NULL
                    BEGIN
                        SET @SQL += ' AND Barcodes.Price ' + @PriceOperator + ' ' + CAST(@PriceValue as nvarchar(20));
                    END

                    IF @QuantityOperator IS NOT NULL AND @QuantityValue IS NOT NULL
                    BEGIN
                        SET @SQL += ' AND Quantity ' + @QuantityOperator + ' ' + CAST(@QuantityValue as nvarchar(20));
                    END

                    SET @SQL += ' ORDER BY Barcodes.Id 
                                  OFFSET ' + CAST(@startOffset as nvarchar(10)) + ' ROWS 
                                  FETCH NEXT ' + CAST(@pageSize as nvarchar(10)) + ' ROWS ONLY;';

                    EXEC sp_executesql @SQL;
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

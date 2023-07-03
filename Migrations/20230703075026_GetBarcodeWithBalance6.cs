using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBarcodeWithBalance6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

                      var createProcSql = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetBarcodeWithBalance](
                @OrganizationId int = NULL,
                @BranchId INT = NULL,
                @Name nvarchar(255) = NULL,
                @PriceOperator nvarchar(1) = NULL,
                @PriceValue decimal(18,2) = NULL,
                @QuantityOperator nvarchar(1) = NULL,
                @QuantityValue decimal(18,2) = NULL,
                @pageNumber INT = 1,
                @pageSize INT = 5
            )
            AS
            BEGIN
                SET NOCOUNT ON;

                DECLARE @startOffset INT = (@pageNumber - 1) * @pageSize;

                DECLARE @SQL nvarchar(MAX) = N'
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
                    SET @SQL += ' AND (CAST(ISNULL((SELECT SUM(Goodsin.Quantity) FROM Goodsin WHERE Goodsin.BarcodeId = Barcodes.Id), 0) - 
                        ISNULL((SELECT SUM(GoodsOut.Quantity) FROM GoodsOut WHERE GoodsOut.BarcodeId = Barcodes.Id), 0) AS REAL)) ' + @QuantityOperator + ' ' + CAST(@QuantityValue as nvarchar(20));
                END

                SET @SQL += ' ORDER BY Barcodes.Id 
                    OFFSET ' + CAST(@startOffset as nvarchar(10)) + ' ROWS 
                    FETCH NEXT ' + CAST(@pageSize as nvarchar(10)) + ' ROWS ONLY;';

                DECLARE @params NVARCHAR(MAX) = N'@OrganizationId int,
                                                  @BranchId int,
                                                  @Name nvarchar(255),
                                                  @PriceOperator nvarchar(1),
                                                  @PriceValue decimal(18,2),
                                                  @QuantityOperator nvarchar(1),
                                                  @QuantityValue decimal(18,2),
                                                  @startOffset int,
                                                  @pageSize int';

                EXEC sp_executesql @SQL, @params, @OrganizationId = @OrganizationId, @BranchId = @BranchId, @Name = @Name, @PriceOperator = @PriceOperator, @PriceValue = @PriceValue, @QuantityOperator = @QuantityOperator, @QuantityValue = @QuantityValue, @startOffset = @startOffset, @pageSize = @pageSize;
            END
            ";

            migrationBuilder.Sql(createProcSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                  var dropProcSql = "DROP PROCEDURE [dbo].[GetBarcodeWithBalance]";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

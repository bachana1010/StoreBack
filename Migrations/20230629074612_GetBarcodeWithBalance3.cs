using Microsoft.EntityFrameworkCore.Migrations;

namespace StoreBack.Migrations
{
    public partial class GetBarcodeWithBalance3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.GetBarcodeWithBalance'))
                    DROP PROCEDURE dbo.GetBarcodeWithBalance
                GO

                SET ANSI_NULLS ON
                SET QUOTED_IDENTIFIER ON

                CREATE PROCEDURE [dbo].[GetBarcodeWithBalance](
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

                    EXEC sp_executesql @SQL;
                END
                GO";

            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.GetBarcodeWithBalance')) DROP PROCEDURE dbo.GetBarcodeWithBalance";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

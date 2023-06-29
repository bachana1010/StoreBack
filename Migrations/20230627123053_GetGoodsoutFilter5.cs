using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetGoodsoutFilter5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                  migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[GetGoodsOut]
                @OrganizationId int,
                @BranchId int = NULL,
                @PageNumber int = 1,
                @PageSize int = 5,
                @QuantityOperator nvarchar(1) = NULL,
                @QuantityValue decimal(18,2) = NULL,
                @DateFrom datetime2 = NULL,
                @DateTo datetime2 = NULL
            AS
            BEGIN
                DECLARE @StartRow int;
                SET @StartRow = (@PageNumber - 1) * @PageSize;

                DECLARE @SQL nvarchar(4000) = N'
                SELECT 
                    G.Id, 
                    G.OutDate, 
                    G.Quantity,
                    B.BrancheName AS BranchName, 
                    U.Username AS OperatorUserName, 
                    BC.Name AS BarcodeName,
                    COUNT(*) OVER() as TotalCount
                FROM GoodsOut G
                INNER JOIN Branches B ON G.BranchId = B.Id
                INNER JOIN Users U ON G.OperatorUserId = U.Id
                INNER JOIN Barcodes BC ON G.BarcodeId = BC.Id
                WHERE G.OrganizationId = ' + CAST(@OrganizationId as nvarchar(10));

                IF @BranchId IS NOT NULL
                BEGIN
                    SET @SQL += ' AND G.BranchId = ' + CAST(@BranchId as nvarchar(10));
                END

                IF @QuantityOperator IS NOT NULL AND @QuantityValue IS NOT NULL
                BEGIN
                    SET @SQL += ' AND G.Quantity ' + @QuantityOperator + ' ' + CAST(@QuantityValue as nvarchar(20));
                END

                IF @DateFrom IS NOT NULL AND @DateTo IS NOT NULL
                BEGIN
                    SET @SQL += ' AND G.OutDate BETWEEN ''' + CONVERT(nvarchar(20), @DateFrom, 120) + ''' AND ''' + CONVERT(nvarchar(20), @DateTo, 120) + '''';
                END
                ELSE IF @DateFrom IS NOT NULL
                BEGIN
                    SET @SQL += ' AND G.OutDate >= ''' + CONVERT(nvarchar(20), @DateFrom, 120) + '''';
                END
                ELSE IF @DateTo IS NOT NULL
                BEGIN
                    SET @SQL += ' AND G.OutDate <= ''' + CONVERT(nvarchar(20), @DateTo, 120) + '''';
                END

                SET @SQL += ' ORDER BY G.Id OFFSET ' + CAST(@StartRow as nvarchar(10)) + ' ROWS FETCH NEXT ' + CAST(@PageSize as nvarchar(10)) + ' ROWS ONLY;';

                EXEC sp_executesql @SQL;
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                            migrationBuilder.Sql("DROP PROCEDURE GetGoodsOut");

        }
    }
}

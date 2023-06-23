using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetGodsOutPagination6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


migrationBuilder.Sql(@"
    CREATE OR ALTER PROCEDURE [dbo].[GetGoodsOut]
        @OrganizationId int,
        @BranchId int = NULL, -- Make BranchId optional
        @PageNumber int = 1,
        @PageSize int = 5 -- Default page size is 10
    AS
    BEGIN
        -- Calculate starting row number
        DECLARE @StartRow INT = (@PageNumber - 1) * @PageSize;
        DECLARE @TotalCount INT;

        -- Calculate total count
        SELECT @TotalCount = COUNT(*) FROM GoodsOut;
        
        -- Return paginated results
        SELECT 
            G.Id, 
            G.OutDate, 
            G.Quantity,
            B.BrancheName AS BrancheName, 
            U.Username AS OperatorUserName, 
            BC.Name AS BarcodeName,
            @TotalCount as TotalCount
        FROM GoodsOut G
        INNER JOIN Branches B ON G.BranchId = B.Id
        INNER JOIN Users U ON G.OperatorUserId = U.Id
        INNER JOIN Barcodes BC ON G.BarcodeId = BC.Id
        WHERE G.OrganizationId = @OrganizationId
        AND (@BranchId IS NULL OR G.BranchId = @BranchId)
        ORDER BY Id
        OFFSET @StartRow ROWS 
        FETCH NEXT @PageSize ROWS ONLY;
    END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
   migrationBuilder.Sql(@"
                DROP PROCEDURE IF EXISTS [dbo].[GetGoodsOut]
            ");

        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class getGoddsinPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE [dbo].[FGetGoodsIn]
                    @OrganizationId int,
                    @BranchId int = NULL, -- Make BranchId optional
                    @PageNumber int = 1,
                    @PageSize int = 5
                AS
                BEGIN
                    -- Calculating the starting number
                    DECLARE @StartRow int;
                    SET @StartRow = (@PageNumber - 1) * @PageSize;
                    DECLARE @TotalCount int;

                    -- If both OrganizationId and BranchId are provided
                    IF @BranchId IS NOT NULL
                    BEGIN
                        SELECT @TotalCount = COUNT(*)
                        FROM Goodsin G
                        WHERE G.OrganizationId = @OrganizationId AND G.BranchId = @BranchId;

                        SELECT 
                            G.Id, 
                            G.EntryDate, 
                            G.Quantity,
                            B.BrancheName AS BranchName, 
                            U.Username AS OperatorUserName, 
                            BC.Name AS BarcodeName,
                            @TotalCount as TotalCount
                        FROM Goodsin G
                        INNER JOIN Branches B ON G.BranchId = B.Id
                        INNER JOIN Users U ON G.OperatorUserId = U.Id
                        INNER JOIN Barcodes BC ON G.BarcodeId = BC.Id
                        WHERE G.OrganizationId = @OrganizationId AND G.BranchId = @BranchId
                        ORDER BY G.Id
                        OFFSET @StartRow ROWS 
                        FETCH NEXT @PageSize ROWS ONLY;
                    END
                    -- If only OrganizationId is provided
                    ELSE
                    BEGIN
                        SELECT @TotalCount = COUNT(*)
                        FROM Goodsin G
                        WHERE G.OrganizationId = @OrganizationId;

                        SELECT 
                            G.Id, 
                            G.EntryDate, 
                            G.Quantity,
                            B.BrancheName AS BranchName, 
                            U.Username AS OperatorUserName, 
                            BC.Name AS BarcodeName,
                            @TotalCount as TotalCount
                        FROM Goodsin G
                        INNER JOIN Branches B ON G.BranchId = B.Id
                        INNER JOIN Users U ON G.OperatorUserId = U.Id
                        INNER JOIN Barcodes BC ON G.BarcodeId = BC.Id
                        WHERE G.OrganizationId = @OrganizationId
                        ORDER BY G.Id
                        OFFSET @StartRow ROWS 
                        FETCH NEXT @PageSize ROWS ONLY;
                    END
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE GetGoodsIn");

        }
    }
}

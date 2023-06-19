using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class CreateGetGoodsInProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[GetGoodsIn]
                @OrganizationId int,
                @BranchId int = NULL -- Make BranchId optional
            AS
            BEGIN
                -- If both OrganizationId and BranchId are provided
                IF @BranchId IS NOT NULL
                BEGIN
                    SELECT 
                        G.Id, 
                        G.EntryDate, 
                        G.Quantity,
                        B.BrancheName AS BranchName, 
                        U.Username AS OperatorUserName, 
                        BC.Name AS BarcodeName
                    FROM Goodsin G
                    INNER JOIN Branches B ON G.BranchId = B.Id
                    INNER JOIN Users U ON G.OperatorUserId = U.Id
                    INNER JOIN Barcodes BC ON G.BarcodeId = BC.Id
                    WHERE G.OrganizationId = @OrganizationId AND G.BranchId = @BranchId;
                END
                -- If only OrganizationId is provided
                ELSE
                BEGIN
                    SELECT 
                        G.Id, 
                        G.EntryDate, 
                        G.Quantity,
                        B.BrancheName AS BranchName, 
                        U.Username AS OperatorUserName, 
                        BC.Name AS BarcodeName
                    FROM Goodsin G
                    INNER JOIN Branches B ON G.BranchId = B.Id
                    INNER JOIN Users U ON G.OperatorUserId = U.Id
                    INNER JOIN Barcodes BC ON G.BarcodeId = BC.Id
                    WHERE G.OrganizationId = @OrganizationId;
                END
            END
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.Sql(@"
            DROP PROCEDURE IF EXISTS [dbo].[GetGoodsIn]
        ");

        }
    }
}

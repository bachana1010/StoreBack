using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class CreateMakeGoodsInProcedure1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        var procedure = @"
                CREATE OR ALTER PROCEDURE [dbo].[MakeGoodsIn]
                    @OrganizationId int,
                    @BranchId int,
                    @OperatorUserId int,
                    @Barcode nvarchar(MAX),
                    @Name nvarchar(MAX),
                    @Price Real,
                    @Unit nvarchar(MAX),
                    @Quantity real
                AS
                BEGIN
                    DECLARE @BarcodeId int;
                    DECLARE @RC int;

                    EXECUTE @RC = [dbo].[CreateOrUpdateBarcode] 
                        @OrganizationId
                        ,@BranchId
                        ,@Barcode
                        ,@Name
                        ,@Price
                        ,@Unit
                        ,@BarcodeId OUTPUT;

                    INSERT INTO Goodsin (BranchId, EntryDate, Quantity, OperatorUserId, OrganizationId, BarcodeId)
                    VALUES (@BranchId, GETDATE(), @Quantity, @OperatorUserId, @OrganizationId, @BarcodeId);
                END";

            migrationBuilder.Sql(procedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[MakeGoodsIn]");

        }
    }
}

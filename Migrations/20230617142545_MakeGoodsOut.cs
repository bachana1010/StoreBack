using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class MakeGoodsOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        var makeGoodsOutProcedure = @"
            CREATE PROCEDURE MakeGoodsOut
                @Barcode NVARCHAR(100),
                @OutQuantity FLOAT,
                @BranchId INT,
                @OperatorUserId INT,
                @OrganizationId INT
            AS
            BEGIN
                SET NOCOUNT ON;

                DECLARE @BarcodeId INT;
                SELECT @BarcodeId = Id FROM dbo.Barcodes WHERE Barcode = @Barcode;

                INSERT INTO dbo.GoodsOut(BranchId, OutDate, Quantity, OperatorUserId, OrganizationId, BarcodeId)
                VALUES (@BranchId, GETDATE(), @OutQuantity, @OperatorUserId, @OrganizationId, @BarcodeId);

                SELECT SCOPE_IDENTITY() AS NewGoodsOutId;
            END";
        
        migrationBuilder.Sql(makeGoodsOutProcedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.Sql("DROP PROCEDURE MakeGoodsOut");

        }
    }
}

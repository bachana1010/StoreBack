using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class CheckQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            var checkQuantityProcedure = @"
            CREATE PROCEDURE CheckQuantity
                @Barcode NVARCHAR(100),
                @RequiredQuantity FLOAT
            AS
            BEGIN
                SET NOCOUNT ON;

                DECLARE @TotalQuantityIn FLOAT = (SELECT SUM(Quantity) FROM GoodsIn WHERE BarcodeId IN (SELECT Id FROM Barcodes WHERE Barcode = @Barcode));
                DECLARE @TotalQuantityOut FLOAT = (SELECT SUM(Quantity) FROM GoodsOut WHERE BarcodeId IN (SELECT Id FROM Barcodes WHERE Barcode = @Barcode));

                DECLARE @AvailableQuantity FLOAT = @TotalQuantityIn - ISNULL(@TotalQuantityOut, 0);

                IF @AvailableQuantity >= @RequiredQuantity
                    SELECT 1 AS IsAvailable, @AvailableQuantity AS RemainingQuantity;
                ELSE
                    SELECT 0 AS IsAvailable, @AvailableQuantity AS RemainingQuantity;
            END";
        
        migrationBuilder.Sql(checkQuantityProcedure);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        migrationBuilder.Sql("DROP PROCEDURE CheckQuantity");

        }
    }
}

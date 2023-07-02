using Microsoft.EntityFrameworkCore.Migrations;

namespace StoreBack.Migrations
{
    public partial class CreateOrUpdateBarcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var procedure = @"
                CREATE OR ALTER PROCEDURE [dbo].[CreateOrUpdateBarcode]
                    @OrganizationId int,
                    @BranchId int,
                    @Barcode nvarchar(MAX),
                    @Name nvarchar(MAX),
                    @Price Real,
                    @Unit nvarchar(MAX),
                    @BarcodeId INT OUTPUT
                AS
                BEGIN
                    SELECT @BarcodeId = Id 
                    FROM Barcodes
                    WHERE Barcode = @Barcode AND BranchId = @BranchId AND OrganizationId = @OrganizationId;

                    IF (@BarcodeId IS NULL)
                    BEGIN
                        INSERT INTO Barcodes (Barcode, Name, Price, Unit, BranchId, OrganizationId)
                        VALUES (@Barcode, @Name, @Price, @Unit, @BranchId, @OrganizationId);

                        SET @BarcodeId = SCOPE_IDENTITY();
                    END
                    ELSE
                    BEGIN
                        UPDATE Barcodes
                        SET Name = @Name,
                            Price = @Price,
                            Unit = @Unit
                        WHERE Id = @BarcodeId;
                    END

                    SELECT @BarcodeId;
                END";
                
            migrationBuilder.Sql(procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[CreateOrUpdateBarcode]");
        }
    }
}

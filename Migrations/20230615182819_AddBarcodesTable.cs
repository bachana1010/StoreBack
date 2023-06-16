using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "Barcodes",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                Barcode = table.Column<string>(nullable: true),
                Name = table.Column<string>(nullable: true),
                Price = table.Column<float>(nullable: true),
                Unit = table.Column<string>(nullable: true),
                BranchId = table.Column<int>(nullable: true),
                OrganizationId = table.Column<int>(nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Barcodes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Barcodes_Branches_BranchId",
                    column: x => x.BranchId,
                    principalTable: "Branches",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Barcodes_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_BranchId",
                table: "Barcodes",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_OrganizationId",
                table: "Barcodes",
                column: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.DropTable(
            name: "Barcodes");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AddGoodsOutTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
                {
                    migrationBuilder.CreateTable(
                    name: "GoodsOut",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        BranchId = table.Column<int>(nullable: false),
                        OutDate = table.Column<DateTime>(nullable: false),
                        Quantity = table.Column<float>(nullable: false),
                        OperatorUserId = table.Column<int>(nullable: false),
                        OrganizationId = table.Column<int>(nullable: false),
                        BarcodeId = table.Column<int>(nullable: false),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_GoodsOut", x => x.Id);
                        table.ForeignKey(
                            name: "FK_GoodsOut_Branches_BranchId",
                            column: x => x.BranchId,
                            principalTable: "Branches",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                        table.ForeignKey(
                            name: "FK_GoodsOut_Users_OperatorUserId",
                            column: x => x.OperatorUserId,
                            principalTable: "Users",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                        table.ForeignKey(
                            name: "FK_GoodsOut_Organizations_OrganizationId",
                            column: x => x.OrganizationId,
                            principalTable: "Organizations",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                        table.ForeignKey(
                            name: "FK_GoodsOut_Barcodes_BarcodeId",
                            column: x => x.BarcodeId,
                            principalTable: "Barcodes",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_GoodsOut_BranchId",
                    table: "GoodsOut",
                    column: "BranchId");

                migrationBuilder.CreateIndex(
                    name: "IX_GoodsOut_OperatorUserId",
                    table: "GoodsOut",
                    column: "OperatorUserId");

                migrationBuilder.CreateIndex(
                    name: "IX_GoodsOut_OrganizationId",
                    table: "GoodsOut",
                    column: "OrganizationId");

                migrationBuilder.CreateIndex(
                    name: "IX_GoodsOut_BarcodeId",
                    table: "GoodsOut",
                    column: "BarcodeId");
            }

        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoodsOut");
        }
    }
}

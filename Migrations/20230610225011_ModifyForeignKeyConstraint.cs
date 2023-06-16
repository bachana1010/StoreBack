using Microsoft.EntityFrameworkCore.Migrations;

namespace StoreBack.Migrations
{
    public partial class ModifyForeignKeyConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Users_AddedByUserId",
                table: "Branches");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Users_AddedByUserId",
                table: "Branches",
                column: "AddedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Users_AddedByUserId",
                table: "Branches");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Users_AddedByUserId",
                table: "Branches",
                column: "AddedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

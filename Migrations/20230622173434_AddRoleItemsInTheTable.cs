using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleItemsInTheTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Roles ([Key]) VALUES ('administrator')");
            migrationBuilder.Sql("INSERT INTO Roles ([Key]) VALUES ('operator')");
            migrationBuilder.Sql("INSERT INTO Roles ([Key]) VALUES ('manager')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Roles WHERE Key IN ('administrator', 'operator', 'manager')");
        }
    }
}

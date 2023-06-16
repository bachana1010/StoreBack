using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class updategetusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE GetUsers(@OrganizationId Int)
                AS
                BEGIN
                    SELECT * FROM Users 
                    where OrganizationId = @OrganizationId
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE GetUsers");

        }
    }
}

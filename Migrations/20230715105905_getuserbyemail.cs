using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class getuserbyemail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {   
            migrationBuilder.Sql(
                @"CREATE PROCEDURE GetUserByEmail
                    @Email nvarchar(256)
                AS
                BEGIN
                    SELECT TOP 1 * FROM Users WHERE Email = @Email;
                END"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql("DROP PROCEDURE GetUserByEmail");

        }
    }
}

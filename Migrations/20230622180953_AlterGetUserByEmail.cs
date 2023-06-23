using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AlterGetUserByEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
var createProcSql = @"
                CREATE OR ALTER PROCEDURE GetUserByEmail
                    @Email NVARCHAR(MAX)
                AS
                BEGIN
                    SELECT  Users.Id,
                            Users.[OrganizationId]
                        ,Users.[Email]
                        ,Users.[FirstName]
                        ,Users.[LastName]
                        ,Users.[Username]
                        ,Users.[PasswordHash]
                        ,Users.[DeletedAt]
                        ,Users.[BranchId]
                        ,Roles.[Key] as Role
                    FROM Users
                    INNER JOIN Roles ON Users.RoleId = Roles.Id 
                    WHERE Email = @Email;
                END
            ";

            migrationBuilder.Sql(createProcSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
var dropProcSql = "DROP PROCEDURE GetUserByEmail";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

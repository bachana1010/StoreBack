using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AltergetUserByIdProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
 var createProcSql = @"
                CREATE OR ALTER PROCEDURE getUserById
                (
                    @Id INT
                )
                AS
                BEGIN
                    SET NOCOUNT ON;

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
                    WHERE Users.Id = @Id AND DeletedAt IS NULL
                END";

            migrationBuilder.Sql(createProcSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        var dropProcSql = "DROP PROCEDURE getUserById";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

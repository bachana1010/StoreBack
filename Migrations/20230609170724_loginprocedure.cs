using Microsoft.EntityFrameworkCore.Migrations;

namespace StoreBack.Migrations
{
    public partial class loginprocedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                CREATE OR ALTER PROCEDURE LoginUser
                (
                    @Username NVARCHAR(MAX),
                    @PasswordHash NVARCHAR(MAX)
                )
                AS
                BEGIN
                    SELECT *
                    FROM Users
                    WHERE Username = @Username AND PasswordHash = @PasswordHash;
                END";

            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE LoginUser";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

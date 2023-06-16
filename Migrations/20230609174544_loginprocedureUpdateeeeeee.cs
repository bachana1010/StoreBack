using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class loginprocedureUpdateeeeeee : Migration
    {
             protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                CREATE OR ALTER PROCEDURE GetUserByEmail
                (
                    @Email NVARCHAR(MAX)
                )
                AS
                BEGIN
                    SELECT *
                    FROM Users
                    WHERE Email = @Email;
                END";

            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE GetUserByEmail";
            migrationBuilder.Sql(dropProcSql);
        }
    }
    }


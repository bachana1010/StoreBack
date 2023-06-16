using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class Getuserbyid1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                var sp = @"CREATE OR ALTER PROCEDURE [dbo].[getUserById]
                    @Id INT
                   AS
                   BEGIN
                   SET NOCOUNT ON;
                   SELECT * FROM Users WHERE Id = @Id AND DeletedAt IS NULL
                   END";

        migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                var sp = @"DROP PROCEDURE [dbo].[getUserById]";

                migrationBuilder.Sql(sp);
        }
    }
}

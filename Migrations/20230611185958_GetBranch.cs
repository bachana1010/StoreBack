using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        var sp = @"CREATE PROCEDURE [dbo].[getBranchById]
                    @Id INT
                   AS
                   BEGIN
                   SET NOCOUNT ON;
                   SELECT * FROM Users WHERE Id = @Id
                   END";

        migrationBuilder.Sql(sp);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        var sp = @"DROP PROCEDURE [dbo].[getBranchById]";

        migrationBuilder.Sql(sp);
        }
    }
}

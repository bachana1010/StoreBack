using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBranchesPagination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE GetBranches(@OrganizationId Int, @PageNumber Int, @PageSize Int)
                AS
                BEGIN
                    SELECT * FROM (
                        SELECT *, COUNT(*) OVER () as TotalCount 
                        FROM Branches
                        WHERE OrganizationId = @OrganizationId AND deletedAt IS NULL
                    ) AS Results
                    ORDER BY Id
                    OFFSET (@PageNumber - 1) * @PageSize ROWS 
                    FETCH NEXT @PageSize ROWS ONLY;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("DROP PROCEDURE GetBranches");

        }
    }
}

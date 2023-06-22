using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class getUsers3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE GetBranches(@OrganizationId INT, @PageNumber INT = 1, @PageSize INT = 5)
                AS
                BEGIN
                    -- Calculate total count
                    DECLARE @TotalCount INT;
                    SELECT @TotalCount = COUNT(*) FROM Branches WHERE OrganizationId = @OrganizationId AND deletedAt IS NULL;

                    -- Fetch the required page of data
                    SELECT *, @TotalCount AS TotalCount 
                    FROM Branches 
                    WHERE OrganizationId = @OrganizationId AND deletedAt IS NULL
                    ORDER BY Id
                    OFFSET (@PageNumber - 1) * @PageSize ROWS 
                    FETCH NEXT @PageSize ROWS ONLY
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

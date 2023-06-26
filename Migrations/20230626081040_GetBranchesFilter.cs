using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBranchesFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
               migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE GetBranches(@OrganizationId INT, @PageNumber INT = 1, @PageSize INT = 5, @BrancheName NVARCHAR(MAX) = NULL, @Username NVARCHAR(MAX) = NULL)
                AS
                BEGIN
                    DECLARE @TotalCount INT;

                    SELECT @TotalCount = COUNT(*)
                    FROM Branches
                    INNER JOIN Users ON Branches.AddedByUserId = Users.Id
                    WHERE Branches.OrganizationId = @OrganizationId
                    AND Branches.DeletedAt IS NULL
                    AND (@BrancheName IS NULL OR Branches.BrancheName LIKE '%' + @BrancheName + '%')
                    AND (@Username IS NULL OR Users.Username LIKE '%' + @Username + '%');

                    SELECT Branches.*, @TotalCount AS TotalCount, Users.Username AS AddedByUsername
                    FROM Branches
                    INNER JOIN Users ON Branches.AddedByUserId = Users.Id
                    WHERE Branches.OrganizationId = @OrganizationId 
                    AND Branches.DeletedAt IS NULL
                    AND (@BrancheName IS NULL OR Branches.BrancheName LIKE '%' + @BrancheName + '%')
                    AND (@Username IS NULL OR Users.Username LIKE '%' + @Username + '%')
                    ORDER BY Branches.Id
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

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class getusersfilter10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                 migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE GetUsers(@OrganizationId INT, @PageNumber INT = 1, @PageSize INT = 5, @Username NVARCHAR(MAX) = NULL, @Email NVARCHAR(MAX) = NULL, @Role NVARCHAR(MAX) = NULL)
            AS
            BEGIN
                DECLARE @TotalCount INT;

                SELECT @TotalCount = COUNT(*) 
                FROM Users 
                INNER JOIN Roles ON Users.RoleId = Roles.Id
                WHERE OrganizationId = @OrganizationId AND deletedAt IS NULL
                AND (@Username IS NULL OR Username LIKE '%' + @Username + '%')
                AND (@Email IS NULL OR Email LIKE '%' + @Email + '%')
                AND (@Role IS NULL OR Roles.[Key] = @Role);

                SELECT *, @TotalCount AS TotalCount, Roles.[Key] as Role
                FROM Users 
                INNER JOIN Roles ON Users.RoleId = Roles.Id
                WHERE OrganizationId = @OrganizationId  AND deletedAt IS NULL
                AND (@Username IS NULL OR Username LIKE '%' + @Username + '%')
                AND (@Email IS NULL OR Email LIKE '%' + @Email + '%')
                AND (@Role IS NULL OR Roles.[Key] = @Role)
                ORDER BY Users.Id
                OFFSET (@PageNumber - 1) * @PageSize ROWS 
                FETCH NEXT @PageSize ROWS ONLY;
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

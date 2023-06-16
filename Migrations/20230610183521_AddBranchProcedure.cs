using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql(@"
                CREATE PROCEDURE AddBranch
                    @OrganizationId int,
                    @BrancheName nvarchar(50),
                    @AddedByUserId int
                AS
                BEGIN
                    INSERT INTO Branches (OrganizationId, BrancheName, AddedByUserId)
                    VALUES (@OrganizationId, @BrancheName, @AddedByUserId)
                    
                    SELECT SCOPE_IDENTITY()
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE AddBranch");
        }
    }
}

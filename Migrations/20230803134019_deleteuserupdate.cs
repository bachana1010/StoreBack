using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class deleteuserupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE DeleteBranch
                    @Id int

                AS
                BEGIN
                    -- Soft delete the users associated with this branch
                    UPDATE Users
                    SET DeletedAt = GETDATE()
                    WHERE BranchId = @Id;                

                    -- Soft delete the branch
                    UPDATE Branches
                    SET DeletedAt = GETDATE()
                    WHERE Id = @Id;
                END
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql("DROP PROCEDURE deleteuserupdate");


        }
    }
}

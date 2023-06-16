using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE DeleteBranch
                    @Id int

                AS
                BEGIN
                    UPDATE Users
                    SET DeletedAt = GETDATE()
                    WHERE Id = @Id                
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE DeleteBranch");

        }
    }
}

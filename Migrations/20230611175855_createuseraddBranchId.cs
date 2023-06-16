using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class createuseraddBranchId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE CreateUser
                    @OrganizationId int,
                    @Email nvarchar(50),
                    @FirstName nvarchar(50),
                    @LastName nvarchar(50),
                    @Username nvarchar(50),
                    @PasswordHash nvarchar(50),
                    @Role nvarchar(50),
                    @BranchId int = NULL,
                    @UserId int OUTPUT


                AS
                BEGIN
                    INSERT INTO Users (OrganizationId, Email, FirstName, LastName, Username, PasswordHash, Role,BranchId)
                    VALUES (@OrganizationId, @Email, @FirstName, @LastName, @Username, @PasswordHash, @Role, @BranchId)
                    SET @UserId = SCOPE_IDENTITY()
    
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE CreateUser");

        }
    }
}

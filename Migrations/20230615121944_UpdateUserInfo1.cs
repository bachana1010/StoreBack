using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserInfo1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.Sql(@"
                        CREATE OR ALTER PROCEDURE UpdateUserInfo
                            @Id int,
                            @FirstName NVARCHAR(50),
                            @LastName NVARCHAR(50),
                            @Email NVARCHAR(50),
                            @Username NVARCHAR(50),
                            @PasswordHash NVARCHAR(50) = NULL,
                            @Role nvarchar(50)
                        AS
                        BEGIN
                            UPDATE Users
                            SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Username = @Username,            
                             PasswordHash = ISNULL(NULLIF(@PasswordHash, ''), PasswordHash), 
                            Role = @Role
                            WHERE Id = @Id;
                        END
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE UpdateUserInfo");

        }
    }
}

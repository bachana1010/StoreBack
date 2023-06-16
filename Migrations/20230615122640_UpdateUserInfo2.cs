using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserInfo2 : Migration
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
                                @PasswordHash nvarchar(50),
                                @Role nvarchar(50)
                        AS
                        BEGIN
                            IF @PasswordHash IS NOT NULL
                            BEGIN
                                UPDATE Users
                                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Username = @Username, PasswordHash = @PasswordHash, Role = @Role
                                WHERE Id = @Id;
                            END
                            ELSE
                            BEGIN
                                UPDATE Users
                                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Username = @Username, Role = @Role
                                WHERE Id = @Id;
                            END
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

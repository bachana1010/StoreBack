using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserInfo4 : Migration
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
                                @PasswordHash nvarchar(MAX) = NULL
                        AS
                        BEGIN
                            IF @PasswordHash IS NOT NULL
                            BEGIN
                                UPDATE Users
                                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Username = @Username, PasswordHash = @PasswordHash
                                WHERE Id = @Id;
                            END
                            ELSE
                            BEGIN
                                UPDATE Users
                                SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Username = @Username
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

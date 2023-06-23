using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AlterCreateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
var createProcSql = @"
                CREATE OR ALTER PROCEDURE CreateUser
                    @OrganizationId int,
                    @Email nvarchar(50),
                    @FirstName nvarchar(50),
                    @LastName nvarchar(50),
                    @Username nvarchar(50),
                    @passwordHash NVARCHAR(MAX),
                    @Role nvarchar(50),
                    @BranchId int = NULL,
                    @UserId int OUTPUT
                AS
                BEGIN
                    DECLARE @RoleId INT;

                    SET @roleId = (SELECT [Id] FROM Roles WHERE [Key] = @Role)

                    INSERT INTO Users (OrganizationId, Email, FirstName, LastName, Username, PasswordHash, RoleId,BranchId)
                    VALUES (@OrganizationId, @Email, @FirstName, @LastName, @Username, @PasswordHash, @RoleId, @BranchId)
                    SET @UserId = SCOPE_IDENTITY()
    
                END
            ";

            migrationBuilder.Sql(createProcSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
var dropProcSql = "DROP PROCEDURE CreateUser";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class AlterRegisterOrganizationAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                CREATE OR ALTER PROCEDURE RegisterOrganizationAndUser
                (
                    @organizationName NVARCHAR(MAX),
                    @address NVARCHAR(MAX),
                    @orgEmail NVARCHAR(MAX),
                    @firstName NVARCHAR(MAX),
                    @lastName NVARCHAR(MAX),
                    @userName NVARCHAR(MAX),
                    @passwordHash NVARCHAR(MAX),
                    @userId INT OUTPUT
                )
                AS
                BEGIN
                    DECLARE @organizationId INT;
                    DECLARE @RoleId INT;

                    INSERT INTO Organizations(Name, Address, Email)
                    VALUES (@organizationName, @address, @orgEmail);
                    
                    SET @organizationId = SCOPE_IDENTITY();

                    SET @roleId = (SELECT [Id] FROM Roles WHERE [Key] = 'administrator')

                    INSERT INTO Users(OrganizationId, Email, FirstName, LastName, Username, PasswordHash,RoleId)
                    VALUES (@organizationId, @orgEmail, @firstName, @lastName, @userName, @passwordHash,@roleId);

                    SET @userId = SCOPE_IDENTITY();

                    SELECT @userId;
                END";

            migrationBuilder.Sql(createProcSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        var dropProcSql = "DROP PROCEDURE RegisterOrganizationAndUser";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

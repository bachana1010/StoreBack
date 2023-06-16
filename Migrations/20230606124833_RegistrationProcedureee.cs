using Microsoft.EntityFrameworkCore.Migrations;

namespace StoreBack.Migrations
{
    public partial class RegistrationProcedureee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                CREATE PROCEDURE RegisterOrganizationAndUser
                (
                    @organizationName NVARCHAR(MAX),
                    @address NVARCHAR(MAX),
                    @orgEmail NVARCHAR(MAX),
                    @firstName NVARCHAR(MAX),
                    @lastName NVARCHAR(MAX),
                    @userName NVARCHAR(MAX),
                    @passwordHash NVARCHAR(MAX)
                )
                AS
                BEGIN
                    DECLARE @organizationId INT;
                    
                    INSERT INTO Organization(Name, Address, Email)
                    VALUES (@organizationName, @address, @orgEmail);
                    
                    SET @organizationId = SCOPE_IDENTITY();

                    INSERT INTO Userinfo(OrganizationId, Email, FirstName, LastName, Username, PasswordHash)
                    VALUES (@organizationId, @orgEmail, @firstName, @lastName, @userName, @passwordHash);
                END";

            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE RegisterOrganizationAndUser";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}

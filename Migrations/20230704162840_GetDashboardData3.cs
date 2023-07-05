using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetDashboardData3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                var procedure = @"CREATE OR ALTER PROCEDURE [dbo].[GetDashboardData]
                    @OrganizationId INT
                AS
                BEGIN
                    DECLARE @ManagerRoleId INT;
                    DECLARE @OperatorRoleId INT;

                    SET @ManagerRoleId = (SELECT [Id] FROM Roles WHERE [Key] = 'manager')
                    SET @OperatorRoleId = (SELECT [Id] FROM Roles WHERE [Key] = 'operator')

                    SELECT 
                        (SELECT COUNT(*) FROM Users WHERE (RoleId = @ManagerRoleId OR RoleId = @OperatorRoleId) AND OrganizationId = @OrganizationId AND DeletedAt IS NULL) AS UserCount,
                        (SELECT COUNT(*) FROM Users WHERE RoleId = @ManagerRoleId AND OrganizationId = @OrganizationId AND DeletedAt IS NULL) AS ManagerCount,
                        (SELECT COUNT(*) FROM Users WHERE RoleId = @OperatorRoleId AND OrganizationId = @OrganizationId AND DeletedAt IS NULL) AS OperatorCount,
                        (SELECT COUNT(*) FROM GoodsIn WHERE OrganizationId = @OrganizationId) AS GoodsInCount,
                        (SELECT COUNT(*) FROM GoodsOut WHERE OrganizationId = @OrganizationId ) AS GoodsOutCount,
                        (SELECT COUNT(DISTINCT Id) FROM Barcodes WHERE OrganizationId = @OrganizationId) AS ProductCount,
                        (SELECT COUNT(*) FROM Branches WHERE OrganizationId = @OrganizationId AND DeletedAt IS NULL) AS BranchCount
                END";

                migrationBuilder.Sql(procedure);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                        migrationBuilder.Sql("DROP PROCEDURE [dbo].[GetDashboardData]");

        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace StoreBack.Migrations
{
    public partial class GetDashboardData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var procedure = @"CREATE OR ALTER PROCEDURE [dbo].[GetDashboardData]
                                @OrganizationId INT
                            AS
                            BEGIN
                                SELECT 
                                    (SELECT COUNT(*) FROM Users WHERE RoleId = 1 AND OrganizationId = @OrganizationId) AS UserCount,
                                    (SELECT COUNT(*) FROM Users WHERE RoleId = 2 AND OrganizationId = @OrganizationId) AS OperatorCount,
                                    (SELECT COUNT(*) FROM Users WHERE RoleId = 3 AND OrganizationId = @OrganizationId) AS ManagerCount,
                                    (SELECT COUNT(*) FROM GoodsIn WHERE OrganizationId = @OrganizationId) AS GoodsInCount,
                                    (SELECT COUNT(*) FROM GoodsOut WHERE OrganizationId = @OrganizationId) AS GoodsOutCount,
                                    (SELECT COUNT(DISTINCT Id) FROM Barcodes WHERE OrganizationId = @OrganizationId) AS ProductCount,
                                    (SELECT COUNT(*) FROM Branches WHERE OrganizationId = @OrganizationId) AS BranchCount
                            END";

            migrationBuilder.Sql(procedure);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[GetDashboardData]");
        }
    }
}

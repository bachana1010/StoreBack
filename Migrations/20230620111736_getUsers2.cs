﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class getUsers2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE GetUsers(@OrganizationId INT, @PageNumber INT = 1, @PageSize INT = 5)
            AS
            BEGIN
                SELECT * 
                FROM Users 
                WHERE OrganizationId = @OrganizationId
                ORDER BY Id
                OFFSET (@PageNumber - 1) * @PageSize ROWS 
                FETCH NEXT @PageSize ROWS ONLY
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE GetUsers");

        }
    }
}

﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBack.Migrations
{
    /// <inheritdoc />
    public partial class GetBranchesFixedProblemAddnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE GetBranches(@OrganizationId Int)
                AS
                BEGIN
                    SELECT * FROM Branches 
                    where OrganizationId = @OrganizationId AND deletedAt IS NULL
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE GetBranches");

        }
    }
}

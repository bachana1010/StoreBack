// using Microsoft.EntityFrameworkCore.Migrations;

// namespace StoreBack.Migrations
// {
//     public partial class createUser : Migration
//     {
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.Sql(@"
//                 CREATE PROCEDURE CreateUser
//                     @OrganizationId int,
//                     @Email nvarchar(50),
//                     @FirstName nvarchar(50),
//                     @LastName nvarchar(50),
//                     @Username nvarchar(50),
//                     @PasswordHash nvarchar(50),
//                     @Role nvarchar(50),
                    

//                 AS
//                 BEGIN
//                     INSERT INTO Users (OrganizationId, Email, FirstName, LastName, Username, PasswordHash, Role)
//                     VALUES (@OrganizationId, @Email, @FirstName, @LastName, @Username, @PasswordHash, @Role)
//                 END
//             ");
//         }

//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.Sql("DROP PROCEDURE CreateUser");
//         }
//     }
// }

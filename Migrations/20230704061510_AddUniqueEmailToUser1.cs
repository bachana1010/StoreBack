﻿// using Microsoft.EntityFrameworkCore.Migrations;

// #nullable disable

// namespace StoreBack.Migrations
// {
//     /// <inheritdoc />
//     public partial class AddUniqueEmailToUser1 : Migration
//     {
//         /// <inheritdoc />
//         protected override void Up(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.CreateIndex(
//                 name: "IX_Users_Email",
//                 table: "Users",
//                 column: "Email",
//                 unique: true);
//         }

//         /// <inheritdoc />
//         protected override void Down(MigrationBuilder migrationBuilder)
//         {
//             migrationBuilder.DropIndex(
//                 name: "IX_Users_Email",
//                 table: "Users");
//         }
//     }
// }
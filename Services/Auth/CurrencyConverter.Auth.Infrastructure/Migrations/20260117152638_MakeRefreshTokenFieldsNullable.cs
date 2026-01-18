using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CurrencyConverter.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeRefreshTokenFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b730da0c-a631-479d-b19d-8d84ed27665b"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d049f131-0ed3-482d-94cc-cf4da201dbeb"));

            migrationBuilder.AlterColumn<string>(
                name: "RevokedByIp",
                table: "RefreshTokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                table: "RefreshTokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("25c48e5a-c596-4eb6-a902-17b1bbd1734d"), "Administrator with full access", "Admin" },
                    { new Guid("a2a52e56-d9b8-4de7-b753-0cfe1bcf6d37"), "Standard user with basic access", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("25c48e5a-c596-4eb6-a902-17b1bbd1734d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a2a52e56-d9b8-4de7-b753-0cfe1bcf6d37"));

            migrationBuilder.AlterColumn<string>(
                name: "RevokedByIp",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("b730da0c-a631-479d-b19d-8d84ed27665b"), "Administrator with full access", "Admin" },
                    { new Guid("d049f131-0ed3-482d-94cc-cf4da201dbeb"), "Standard user with basic access", "User" }
                });
        }
    }
}

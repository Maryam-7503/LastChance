using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "AccountType", "Address", "DateOfBirth", "NationalId", "PasswordHash", "PhoneNumber" },
                values: new object[] { null, null, null, null, "$2a$11$MHvRggquioPAWlwAHypNnOW9CvcBdlzI3Q2NbsIiu4R4t2F7CpFB2", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                column: "PasswordHash",
                value: "$2a$11$06u.sv4CzMbAh2nqqFtYCu5rrB/cb3HVQ7du6y.u.QSxyi0zhSGuK");
        }
    }
}

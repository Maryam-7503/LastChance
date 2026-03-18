using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser99 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FailedLoginAttempts", "IsActive", "LockoutEnd", "PasswordHash", "ResetPasswordToken", "ResetPasswordTokenExpiry", "RoleId", "Username" },
                values: new object[] { 99, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@system.com", 0, true, null, "$2a$11$T4enKZZ4Z.V2lJCj8/P29OuNj7kAgXmGqYyHm3cuLyV9SnAzUDrXC", null, null, 1, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99);
        }
    }
}

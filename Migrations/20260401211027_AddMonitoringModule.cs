using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitoringModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                column: "PasswordHash",
                value: "$2a$11$06u.sv4CzMbAh2nqqFtYCu5rrB/cb3HVQ7du6y.u.QSxyi0zhSGuK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99,
                column: "PasswordHash",
                value: "$2a$11$Zi7tUyj4amCIczGG7FIR7.9ef/6ssYa0XE/YxZNMdjMgf1ii6C8fO");
        }
    }
}

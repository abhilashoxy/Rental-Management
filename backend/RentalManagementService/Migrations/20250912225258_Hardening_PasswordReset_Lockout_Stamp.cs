using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalManagementService.Migrations
{
    /// <inheritdoc />
    public partial class Hardening_PasswordReset_Lockout_Stamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "PasswordResetTokens",
                newName: "TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                newName: "IX_PasswordResetTokens_TokenHash");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEndUtc",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LockoutEndUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "PasswordResetTokens",
                newName: "Token");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetTokens_TokenHash",
                table: "PasswordResetTokens",
                newName: "IX_PasswordResetTokens_Token");
        }
    }
}

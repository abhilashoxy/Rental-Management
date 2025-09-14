using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalManagementService.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetToken1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetToken_Users_UserId",
                table: "PasswordResetToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResetToken",
                table: "PasswordResetToken");

            migrationBuilder.RenameTable(
                name: "PasswordResetToken",
                newName: "PasswordResetTokens");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetToken_UserId",
                table: "PasswordResetTokens",
                newName: "IX_PasswordResetTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetToken_Token",
                table: "PasswordResetTokens",
                newName: "IX_PasswordResetTokens_Token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResetTokens",
                table: "PasswordResetTokens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetTokens_Users_UserId",
                table: "PasswordResetTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetTokens_Users_UserId",
                table: "PasswordResetTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResetTokens",
                table: "PasswordResetTokens");

            migrationBuilder.RenameTable(
                name: "PasswordResetTokens",
                newName: "PasswordResetToken");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetToken",
                newName: "IX_PasswordResetToken_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetToken",
                newName: "IX_PasswordResetToken_Token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResetToken",
                table: "PasswordResetToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetToken_Users_UserId",
                table: "PasswordResetToken",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

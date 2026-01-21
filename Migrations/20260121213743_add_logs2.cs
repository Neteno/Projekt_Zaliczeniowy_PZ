using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt_Zaliczeniowy_PZ.Migrations
{
    /// <inheritdoc />
    public partial class add_logs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "VersionLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_VersionLogs_UserId",
                table: "VersionLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VersionLogs_AspNetUsers_UserId",
                table: "VersionLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VersionLogs_AspNetUsers_UserId",
                table: "VersionLogs");

            migrationBuilder.DropIndex(
                name: "IX_VersionLogs_UserId",
                table: "VersionLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "VersionLogs");
        }
    }
}

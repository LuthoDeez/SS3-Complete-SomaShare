using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomaShare.Migrations
{
    /// <inheritdoc />
    public partial class FixUserNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_AspNetUsers_ApplicationUserId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_ApplicationUserId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Offers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Offers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ApplicationUserId",
                table: "Offers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_AspNetUsers_ApplicationUserId",
                table: "Offers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}

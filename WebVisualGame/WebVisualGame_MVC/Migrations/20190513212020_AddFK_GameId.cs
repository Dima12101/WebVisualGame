using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame_MVC.Migrations
{
    public partial class AddFK_GameId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_GameId",
                table: "Images",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Games_GameId",
                table: "Images",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Games_GameId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_GameId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Images");
        }
    }
}

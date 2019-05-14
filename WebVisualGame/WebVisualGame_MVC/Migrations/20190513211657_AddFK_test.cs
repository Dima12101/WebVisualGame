using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame_MVC.Migrations
{
    public partial class AddFK_test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "PointDialogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PointDialogs_ImageId",
                table: "PointDialogs",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PointDialogs_Images_ImageId",
                table: "PointDialogs",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointDialogs_Images_ImageId",
                table: "PointDialogs");

            migrationBuilder.DropIndex(
                name: "IX_PointDialogs_ImageId",
                table: "PointDialogs");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "PointDialogs");
        }
    }
}

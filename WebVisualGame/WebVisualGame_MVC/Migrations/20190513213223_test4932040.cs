using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame_MVC.Migrations
{
    public partial class test4932040 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointDialogs_Images_ImageId",
                table: "PointDialogs");

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "PointDialogs",
                nullable: true,
                oldClrType: typeof(int));

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

            migrationBuilder.AlterColumn<int>(
                name: "ImageId",
                table: "PointDialogs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PointDialogs_Images_ImageId",
                table: "PointDialogs",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

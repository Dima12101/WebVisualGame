using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame_MVC.Migrations
{
    public partial class updateClassGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlIcon",
                table: "Games",
                newName: "PathIcon");

            migrationBuilder.RenameColumn(
                name: "SourceCode",
                table: "Games",
                newName: "PathCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PathIcon",
                table: "Games",
                newName: "UrlIcon");

            migrationBuilder.RenameColumn(
                name: "PathCode",
                table: "Games",
                newName: "SourceCode");
        }
    }
}

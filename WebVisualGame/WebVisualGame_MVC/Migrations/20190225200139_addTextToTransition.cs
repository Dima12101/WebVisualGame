using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame_MVC.Migrations
{
    public partial class addTextToTransition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Transitions",
                maxLength: 400,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Transitions");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame_MVC.Migrations
{
    public partial class Test111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(
				name: "PasswordConfirm",
				table: "Users");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AddColumn<string>(
				name: "PasswordConfirm",
				table: "Users",
				nullable: true);
		}
    }
}

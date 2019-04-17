using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame_MVC.Migrations
{
    public partial class addActionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointDialogues");

            migrationBuilder.DropColumn(
                name: "NumberInList",
                table: "Transitions");

            migrationBuilder.AlterColumn<int>(
                name: "KeyСondition",
                table: "Сonditions",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "Type",
                table: "Сonditions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PointDialogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GameId = table.Column<int>(nullable: false),
                    StateNumber = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 300, nullable: false),
                    Background_imageURL = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointDialogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointDialogs_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransitionActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TransitionId = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false),
                    KeyAction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitionActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransitionActions_Transitions_TransitionId",
                        column: x => x.TransitionId,
                        principalTable: "Transitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointDialogActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PointDialogId = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false),
                    KeyAction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointDialogActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointDialogActions_PointDialogs_PointDialogId",
                        column: x => x.PointDialogId,
                        principalTable: "PointDialogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointDialogActions_PointDialogId",
                table: "PointDialogActions",
                column: "PointDialogId");

            migrationBuilder.CreateIndex(
                name: "IX_PointDialogs_GameId",
                table: "PointDialogs",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitionActions_TransitionId",
                table: "TransitionActions",
                column: "TransitionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointDialogActions");

            migrationBuilder.DropTable(
                name: "TransitionActions");

            migrationBuilder.DropTable(
                name: "PointDialogs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Сonditions");

            migrationBuilder.AlterColumn<string>(
                name: "KeyСondition",
                table: "Сonditions",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "NumberInList",
                table: "Transitions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PointDialogues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Background_imageURL = table.Column<string>(maxLength: 100, nullable: false),
                    GameId = table.Column<int>(nullable: false),
                    StateNumber = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointDialogues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointDialogues_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointDialogues_GameId",
                table: "PointDialogues",
                column: "GameId");
        }
    }
}

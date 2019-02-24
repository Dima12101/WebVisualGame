using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame.Migrations
{
    public partial class notNullForFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointDialogue_Games_GameId",
                table: "PointDialogue");

            migrationBuilder.DropForeignKey(
                name: "FK_Transition_Games_GameId",
                table: "Transition");

            migrationBuilder.DropForeignKey(
                name: "FK_Сondition_Transition_TransitionId",
                table: "Сondition");

            migrationBuilder.AlterColumn<int>(
                name: "TransitionId",
                table: "Сondition",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Transition",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "PointDialogue",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PointDialogue_Games_GameId",
                table: "PointDialogue",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transition_Games_GameId",
                table: "Transition",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Сondition_Transition_TransitionId",
                table: "Сondition",
                column: "TransitionId",
                principalTable: "Transition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointDialogue_Games_GameId",
                table: "PointDialogue");

            migrationBuilder.DropForeignKey(
                name: "FK_Transition_Games_GameId",
                table: "Transition");

            migrationBuilder.DropForeignKey(
                name: "FK_Сondition_Transition_TransitionId",
                table: "Сondition");

            migrationBuilder.AlterColumn<int>(
                name: "TransitionId",
                table: "Сondition",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Transition",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "PointDialogue",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_PointDialogue_Games_GameId",
                table: "PointDialogue",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transition_Games_GameId",
                table: "Transition",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Сondition_Transition_TransitionId",
                table: "Сondition",
                column: "TransitionId",
                principalTable: "Transition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame.Migrations
{
    public partial class LastTest : Migration
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Сondition",
                table: "Сondition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transition",
                table: "Transition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointDialogue",
                table: "PointDialogue");

            migrationBuilder.RenameTable(
                name: "Сondition",
                newName: "Сonditions");

            migrationBuilder.RenameTable(
                name: "Transition",
                newName: "Transitions");

            migrationBuilder.RenameTable(
                name: "PointDialogue",
                newName: "PointDialogues");

            migrationBuilder.RenameIndex(
                name: "IX_Сondition_TransitionId",
                table: "Сonditions",
                newName: "IX_Сonditions_TransitionId");

            migrationBuilder.RenameIndex(
                name: "IX_Transition_GameId",
                table: "Transitions",
                newName: "IX_Transitions_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_PointDialogue_GameId",
                table: "PointDialogues",
                newName: "IX_PointDialogues_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Сonditions",
                table: "Сonditions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transitions",
                table: "Transitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointDialogues",
                table: "PointDialogues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PointDialogues_Games_GameId",
                table: "PointDialogues",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transitions_Games_GameId",
                table: "Transitions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Сonditions_Transitions_TransitionId",
                table: "Сonditions",
                column: "TransitionId",
                principalTable: "Transitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointDialogues_Games_GameId",
                table: "PointDialogues");

            migrationBuilder.DropForeignKey(
                name: "FK_Transitions_Games_GameId",
                table: "Transitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Сonditions_Transitions_TransitionId",
                table: "Сonditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Сonditions",
                table: "Сonditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transitions",
                table: "Transitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointDialogues",
                table: "PointDialogues");

            migrationBuilder.RenameTable(
                name: "Сonditions",
                newName: "Сondition");

            migrationBuilder.RenameTable(
                name: "Transitions",
                newName: "Transition");

            migrationBuilder.RenameTable(
                name: "PointDialogues",
                newName: "PointDialogue");

            migrationBuilder.RenameIndex(
                name: "IX_Сonditions_TransitionId",
                table: "Сondition",
                newName: "IX_Сondition_TransitionId");

            migrationBuilder.RenameIndex(
                name: "IX_Transitions_GameId",
                table: "Transition",
                newName: "IX_Transition_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_PointDialogues_GameId",
                table: "PointDialogue",
                newName: "IX_PointDialogue_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Сondition",
                table: "Сondition",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transition",
                table: "Transition",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointDialogue",
                table: "PointDialogue",
                column: "Id");

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
    }
}

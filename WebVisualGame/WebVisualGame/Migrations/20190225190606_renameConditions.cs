using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebVisualGame.Migrations
{
    public partial class renameConditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Сonditions");

            migrationBuilder.CreateTable(
                name: "Conditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TransitionId = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false),
                    KeyСondition = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conditions_Transitions_TransitionId",
                        column: x => x.TransitionId,
                        principalTable: "Transitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conditions_TransitionId",
                table: "Conditions",
                column: "TransitionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conditions");

            migrationBuilder.CreateTable(
                name: "Сonditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KeyСondition = table.Column<int>(nullable: false),
                    TransitionId = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Сonditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Сonditions_Transitions_TransitionId",
                        column: x => x.TransitionId,
                        principalTable: "Transitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Сonditions_TransitionId",
                table: "Сonditions",
                column: "TransitionId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Implementations.RepositoriesEF.Migrations
{
    public partial class InitDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StartDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FinishDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: true),
                    AwaitableMove = table.Column<int>(type: "integer", nullable: true),
                    CheckerCellColor = table.Column<string>(type: "text", nullable: false),
                    FirstPlayerCheckerColor = table.Column<string>(type: "text", nullable: false),
                    OpponentCheckerColor = table.Column<string>(type: "text", nullable: false),
                    NonPlayableCellColor = table.Column<string>(type: "text", nullable: false),
                    FirstPlayerBoardSide = table.Column<int>(type: "integer", nullable: false),
                    SecondPlayerBoardSide = table.Column<int>(type: "integer", nullable: false),
                    Winner = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameId",
                table: "Players",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}

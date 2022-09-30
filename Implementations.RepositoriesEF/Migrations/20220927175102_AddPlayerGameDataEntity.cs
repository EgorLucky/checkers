using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Implementations.RepositoriesEF.Migrations
{
    public partial class AddPlayerGameDataEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotPlayerGameData",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerCode = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotPlayerGameData", x => new { x.PlayerCode, x.GameId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotPlayerGameData");
        }
    }
}

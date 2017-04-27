using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class initialv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "airlines",
                columns: table => new
                {
                    airlineID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    airlineName = table.Column<string>(nullable: true),
                    amotd = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_airlines", x => x.airlineID);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    playerID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    airlineID = table.Column<int>(nullable: true),
                    alevel = table.Column<int>(nullable: false),
                    arank = table.Column<int>(nullable: false),
                    dob = table.Column<DateTime>(nullable: false),
                    isBanned = table.Column<bool>(nullable: false),
                    password = table.Column<string>(nullable: true),
                    posx = table.Column<float>(nullable: false),
                    posy = table.Column<float>(nullable: false),
                    posz = table.Column<float>(nullable: false),
                    username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.playerID);
                    table.ForeignKey(
                        name: "FK_players_airlines_airlineID",
                        column: x => x.airlineID,
                        principalTable: "airlines",
                        principalColumn: "airlineID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bans",
                columns: table => new
                {
                    banID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    banDate = table.Column<DateTime>(nullable: false),
                    banReason = table.Column<string>(nullable: true),
                    playerID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bans", x => x.banID);
                    table.ForeignKey(
                        name: "FK_bans_players_playerID",
                        column: x => x.playerID,
                        principalTable: "players",
                        principalColumn: "playerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bans_playerID",
                table: "bans",
                column: "playerID");

            migrationBuilder.CreateIndex(
                name: "IX_players_airlineID",
                table: "players",
                column: "airlineID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bans");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "airlines");
        }
    }
}

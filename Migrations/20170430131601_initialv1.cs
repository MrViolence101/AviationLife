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
                    AppAuthRank = table.Column<int>(nullable: false),
                    airlineName = table.Column<string>(nullable: true),
                    amotd = table.Column<string>(nullable: true),
                    bankBalance = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_airlines", x => x.airlineID);
                });

            migrationBuilder.CreateTable(
                name: "ranks",
                columns: table => new
                {
                    rankID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    airlineID = table.Column<int>(nullable: true),
                    rankName = table.Column<string>(nullable: true),
                    rankSpot = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ranks", x => x.rankID);
                    table.ForeignKey(
                        name: "FK_ranks_airlines_airlineID",
                        column: x => x.airlineID,
                        principalTable: "airlines",
                        principalColumn: "airlineID",
                        onDelete: ReferentialAction.Restrict);
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
                    cash = table.Column<int>(nullable: false),
                    dob = table.Column<DateTime>(nullable: false),
                    isBanned = table.Column<bool>(nullable: false),
                    isOnline = table.Column<bool>(nullable: false),
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
                name: "airlineAnnouncements",
                columns: table => new
                {
                    announcementID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    airlineID = table.Column<int>(nullable: true),
                    announcement = table.Column<string>(nullable: true),
                    createdByplayerID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_airlineAnnouncements", x => x.announcementID);
                    table.ForeignKey(
                        name: "FK_airlineAnnouncements_airlines_airlineID",
                        column: x => x.airlineID,
                        principalTable: "airlines",
                        principalColumn: "airlineID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_airlineAnnouncements_players_createdByplayerID",
                        column: x => x.createdByplayerID,
                        principalTable: "players",
                        principalColumn: "playerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pendingApplications",
                columns: table => new
                {
                    appID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    DateApplied = table.Column<DateTime>(nullable: false),
                    airlineID = table.Column<int>(nullable: true),
                    applyingPlayerplayerID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pendingApplications", x => x.appID);
                    table.ForeignKey(
                        name: "FK_pendingApplications_airlines_airlineID",
                        column: x => x.airlineID,
                        principalTable: "airlines",
                        principalColumn: "airlineID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pendingApplications_players_applyingPlayerplayerID",
                        column: x => x.applyingPlayerplayerID,
                        principalTable: "players",
                        principalColumn: "playerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bans",
                columns: table => new
                {
                    banID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    adminplayerID = table.Column<int>(nullable: true),
                    banDate = table.Column<DateTime>(nullable: false),
                    banReason = table.Column<string>(nullable: true),
                    playerID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bans", x => x.banID);
                    table.ForeignKey(
                        name: "FK_bans_players_adminplayerID",
                        column: x => x.adminplayerID,
                        principalTable: "players",
                        principalColumn: "playerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bans_players_playerID",
                        column: x => x.playerID,
                        principalTable: "players",
                        principalColumn: "playerID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_airlineAnnouncements_airlineID",
                table: "airlineAnnouncements",
                column: "airlineID");

            migrationBuilder.CreateIndex(
                name: "IX_airlineAnnouncements_createdByplayerID",
                table: "airlineAnnouncements",
                column: "createdByplayerID");

            migrationBuilder.CreateIndex(
                name: "IX_pendingApplications_airlineID",
                table: "pendingApplications",
                column: "airlineID");

            migrationBuilder.CreateIndex(
                name: "IX_pendingApplications_applyingPlayerplayerID",
                table: "pendingApplications",
                column: "applyingPlayerplayerID");

            migrationBuilder.CreateIndex(
                name: "IX_ranks_airlineID",
                table: "ranks",
                column: "airlineID");

            migrationBuilder.CreateIndex(
                name: "IX_bans_adminplayerID",
                table: "bans",
                column: "adminplayerID");

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
                name: "airlineAnnouncements");

            migrationBuilder.DropTable(
                name: "pendingApplications");

            migrationBuilder.DropTable(
                name: "ranks");

            migrationBuilder.DropTable(
                name: "bans");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "airlines");
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class initialv7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_pendingApplications_airlineID",
                table: "pendingApplications",
                column: "airlineID");

            migrationBuilder.CreateIndex(
                name: "IX_pendingApplications_applyingPlayerplayerID",
                table: "pendingApplications",
                column: "applyingPlayerplayerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pendingApplications");
        }
    }
}

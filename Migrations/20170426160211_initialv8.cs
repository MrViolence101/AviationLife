using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class initialv8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_airlineAnnouncements_airlineID",
                table: "airlineAnnouncements",
                column: "airlineID");

            migrationBuilder.CreateIndex(
                name: "IX_airlineAnnouncements_createdByplayerID",
                table: "airlineAnnouncements",
                column: "createdByplayerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "airlineAnnouncements");
        }
    }
}

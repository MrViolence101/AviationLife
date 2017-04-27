using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class initialv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "adminplayerID",
                table: "bans",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bans_adminplayerID",
                table: "bans",
                column: "adminplayerID");

            migrationBuilder.AddForeignKey(
                name: "FK_bans_players_adminplayerID",
                table: "bans",
                column: "adminplayerID",
                principalTable: "players",
                principalColumn: "playerID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bans_players_adminplayerID",
                table: "bans");

            migrationBuilder.DropIndex(
                name: "IX_bans_adminplayerID",
                table: "bans");

            migrationBuilder.DropColumn(
                name: "adminplayerID",
                table: "bans");
        }
    }
}

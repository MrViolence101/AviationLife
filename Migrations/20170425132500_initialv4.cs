using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class initialv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_ranks_airlineID",
                table: "ranks",
                column: "airlineID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ranks");
        }
    }
}

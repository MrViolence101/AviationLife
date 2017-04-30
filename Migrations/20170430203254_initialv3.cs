using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class initialv3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppAuthRank",
                table: "airlines");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppAuthRank",
                table: "airlines",
                nullable: false,
                defaultValue: 0);
        }
    }
}

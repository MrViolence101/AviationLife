using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace core.Migrations
{
    public partial class initialv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "hasAppAuth",
                table: "ranks",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hasAppAuth",
                table: "ranks");
        }
    }
}

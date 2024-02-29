using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheMovieDB.Migrations
{
    public partial class addedActorPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ActorImage",
                table: "Actor",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActorImage",
                table: "Actor");
        }
    }
}

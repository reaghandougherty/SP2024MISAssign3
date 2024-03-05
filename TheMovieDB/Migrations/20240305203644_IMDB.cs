using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheMovieDB.Migrations
{
    public partial class IMDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IMDB",
                table: "Movie",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMDB",
                table: "Movie");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Assignment3.Data.Migrations
{
    public partial class ModifiedCandy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Candy",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Cost",
                table: "Candy",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProductImage",
                table: "Candy",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Candy");

            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "Candy");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Candy",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}

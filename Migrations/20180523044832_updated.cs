using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PlexClone.Migrations
{
    public partial class updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Year",
                table: "Movies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Runtime",
                table: "Movies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "CodecName",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HD",
                table: "Files",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Quality",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "Files",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodecName",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "HD",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Quality",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "Files");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Movies",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Runtime",
                table: "Movies",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

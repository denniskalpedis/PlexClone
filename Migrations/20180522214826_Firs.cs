using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PlexClone.Migrations
{
    public partial class Firs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Libraries_Libraryid",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Movies_Moviesid",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_Moviesid",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Moviesid",
                table: "Files");

            migrationBuilder.AlterColumn<int>(
                name: "Libraryid",
                table: "Files",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Movieid",
                table: "Files",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_Movieid",
                table: "Files",
                column: "Movieid");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Libraries_Libraryid",
                table: "Files",
                column: "Libraryid",
                principalTable: "Libraries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Movies_Movieid",
                table: "Files",
                column: "Movieid",
                principalTable: "Movies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Libraries_Libraryid",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Movies_Movieid",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_Movieid",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Movieid",
                table: "Files");

            migrationBuilder.AlterColumn<int>(
                name: "Libraryid",
                table: "Files",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Moviesid",
                table: "Files",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Files_Moviesid",
                table: "Files",
                column: "Moviesid");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Libraries_Libraryid",
                table: "Files",
                column: "Libraryid",
                principalTable: "Libraries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Movies_Moviesid",
                table: "Files",
                column: "Moviesid",
                principalTable: "Movies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

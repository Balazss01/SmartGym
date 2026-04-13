using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApiBackend.Migrations
{
    /// <inheritdoc />
    public partial class KilepesHozzaadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "KilepesIdopont",
                table: "belepesek",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KilepesIdopont",
                table: "belepesek");
        }
    }
}

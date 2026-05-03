using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApiBackend.Migrations
{
    /// <inheritdoc />
    public partial class ErtesitesekEltavolitasa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ertesitesek");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ertesitesek",
                columns: table => new
                {
                    ertesites_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    datum = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    olvasott = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    tag_id = table.Column<int>(type: "int", nullable: false),
                    uzenet = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ertesitesek", x => x.ertesites_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}

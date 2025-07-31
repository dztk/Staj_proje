using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddBakimHareket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        { 
            migrationBuilder.CreateTable(
                name: "BakimHareket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEmriId = table.Column<int>(type: "int", nullable: false),
                    YapilanIslem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersonelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BakimHareket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BakimHareket_IsEmri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BakimHareket_Personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BakimHareket_IsEmriId",
                table: "BakimHareket",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_BakimHareket_PersonelId",
                table: "BakimHareket",
                column: "PersonelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BakimHareket");
        }
    }
}

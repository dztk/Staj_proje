using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddIsEmri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IsEmri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    AracId = table.Column<int>(type: "int", nullable: true),
                    DurakId = table.Column<int>(type: "int", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    AcilisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KapanisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PersonelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsEmri_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IsEmri_Durak_DurakId",
                        column: x => x.DurakId,
                        principalTable: "Durak",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IsEmri_Personel_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IsEmri_AracId",
                table: "IsEmri",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmri_DurakId",
                table: "IsEmri",
                column: "DurakId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmri_PersonelId",
                table: "IsEmri",
                column: "PersonelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IsEmri");
        }
    }
}

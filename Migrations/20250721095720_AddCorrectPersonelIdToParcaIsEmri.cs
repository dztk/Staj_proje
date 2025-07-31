using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddCorrectPersonelIdToParcaIsEmri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "ParcaIsEmri",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ParcaIsEmri_Personel_PersonelId",
                table: "ParcaIsEmri",
                column: "PersonelId",
                principalTable: "Personel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddForeignKey(
                name: "FK_ParcaIsEmri_Personel_PersonelId",
                table: "ParcaIsEmri",
                column: "PersonelId",
                principalTable: "Personel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddIscilikSuresiToParcaIsEmri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IscilikSuresiSaat",
                table: "ParcaIsEmri",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "ParcaIsEmri",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParcaIsEmri_PersonelId",
                table: "ParcaIsEmri",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParcaIsEmri_Personel_PersonelId",
                table: "ParcaIsEmri",
                column: "PersonelId",
                principalTable: "Personel",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParcaIsEmri_Personel_PersonelId",
                table: "ParcaIsEmri");

            migrationBuilder.DropIndex(
                name: "IX_ParcaIsEmri_PersonelId",
                table: "ParcaIsEmri");

            migrationBuilder.DropColumn(
                name: "IscilikSuresiSaat",
                table: "ParcaIsEmri");

            migrationBuilder.DropColumn(
                name: "PersonelId",
                table: "ParcaIsEmri");
        }
    }
}

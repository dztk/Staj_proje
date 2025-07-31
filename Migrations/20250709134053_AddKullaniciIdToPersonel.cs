using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddKullaniciIdToPersonel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KullaniciId",
                table: "Personel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "Personel");
        }
    }
}

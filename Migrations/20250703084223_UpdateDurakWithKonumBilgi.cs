using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class UpdateDurakWithKonumBilgi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Konum",
                table: "Durak");

            migrationBuilder.AddColumn<double>(
                name: "KonumBilgi_Boylam",
                table: "Durak",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "KonumBilgi_Enlem",
                table: "Durak",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KonumBilgi_Boylam",
                table: "Durak");

            migrationBuilder.DropColumn(
                name: "KonumBilgi_Enlem",
                table: "Durak");

            migrationBuilder.AddColumn<string>(
                name: "Konum",
                table: "Durak",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class UpdateBakimHareket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YapılanIslem",
                table: "BakimHareket",
                newName: "YapilanIslem");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YapilanIslem",
                table: "BakimHareket",
                newName: "YapılanIslem");
        }
    }
}

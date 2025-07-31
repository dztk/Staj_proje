using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddImzaPdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImzaPdf",
                table: "IsEmri",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImzaPdf",
                table: "IsEmri");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddIsEmriParca : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IsEmriParca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParcaId = table.Column<int>(type: "int", nullable: false),
                    IsEmriId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmriParca", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsEmriParca_IsEmri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IsEmriParca_Parca_ParcaId",
                        column: x => x.ParcaId,
                        principalTable: "Parca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IsEmriParca_IsEmriId",
                table: "IsEmriParca",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmriParca_ParcaId",
                table: "IsEmriParca",
                column: "ParcaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IsEmriParca");
        }
    }
}

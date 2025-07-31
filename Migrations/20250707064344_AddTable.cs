using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class AddTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Table",
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
                    table.PrimaryKey("PK_Table", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Table_IsEmri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Table_Parca_ParcaId",
                        column: x => x.ParcaId,
                        principalTable: "Parca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Table_IsEmriId",
                table: "Table",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_Table_ParcaId",
                table: "Table",
                column: "ParcaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Table");
        }
    }
}

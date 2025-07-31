using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proje.Migrations
{
    public partial class RenameTableToParcaIsEmri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IsEmriParca");

            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.CreateTable(
                name: "ParcaIsEmri",
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
                    table.PrimaryKey("PK_ParcaIsEmri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcaIsEmri_IsEmri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParcaIsEmri_Parca_ParcaId",
                        column: x => x.ParcaId,
                        principalTable: "Parca",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParcaIsEmri_IsEmriId",
                table: "ParcaIsEmri",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcaIsEmri_ParcaId",
                table: "ParcaIsEmri",
                column: "ParcaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParcaIsEmri");

            migrationBuilder.CreateTable(
                name: "IsEmriParca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEmriId = table.Column<int>(type: "int", nullable: false),
                    ParcaId = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Table",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsEmriId = table.Column<int>(type: "int", nullable: false),
                    ParcaId = table.Column<int>(type: "int", nullable: false),
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
                name: "IX_IsEmriParca_IsEmriId",
                table: "IsEmriParca",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmriParca_ParcaId",
                table: "IsEmriParca",
                column: "ParcaId");

            migrationBuilder.CreateIndex(
                name: "IX_Table_IsEmriId",
                table: "Table",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_Table_ParcaId",
                table: "Table",
                column: "ParcaId");
        }
    }
}

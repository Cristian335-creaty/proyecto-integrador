using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estudiante.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Problemas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dificultad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputEsperado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputEsperado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CasosAdicionales = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CasoPrueba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entrada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalidaEsperada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasoPrueba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasoPrueba_Problemas_ProblemaId",
                        column: x => x.ProblemaId,
                        principalTable: "Problemas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasoPrueba_ProblemaId",
                table: "CasoPrueba",
                column: "ProblemaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasoPrueba");

            migrationBuilder.DropTable(
                name: "Problemas");
        }
    }
}

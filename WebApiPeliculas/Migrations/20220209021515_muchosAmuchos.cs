using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiPeliculas.Migrations
{
    public partial class muchosAmuchos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PeliculasActores",
                columns: table => new
                {
                    ActorID = table.Column<int>(type: "int", nullable: false),
                    PeliculaID = table.Column<int>(type: "int", nullable: false),
                    Personaje = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeliculasActores", x => new { x.ActorID, x.PeliculaID });
                    table.ForeignKey(
                        name: "FK_PeliculasActores_Actores_ActorID",
                        column: x => x.ActorID,
                        principalTable: "Actores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeliculasActores_Peliculas_PeliculaID",
                        column: x => x.PeliculaID,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeliculasGeneros",
                columns: table => new
                {
                    GeneroID = table.Column<int>(type: "int", nullable: false),
                    PeliculaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeliculasGeneros", x => new { x.GeneroID, x.PeliculaID });
                    table.ForeignKey(
                        name: "FK_PeliculasGeneros_Generos_GeneroID",
                        column: x => x.GeneroID,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeliculasGeneros_Peliculas_PeliculaID",
                        column: x => x.PeliculaID,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasActores_PeliculaID",
                table: "PeliculasActores",
                column: "PeliculaID");

            migrationBuilder.CreateIndex(
                name: "IX_PeliculasGeneros_PeliculaID",
                table: "PeliculasGeneros",
                column: "PeliculaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeliculasActores");

            migrationBuilder.DropTable(
                name: "PeliculasGeneros");
        }
    }
}

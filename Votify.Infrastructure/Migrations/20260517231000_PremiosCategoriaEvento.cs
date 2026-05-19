using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Infrastructure.Migrations
{
    public partial class PremiosCategoriaEvento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Premio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_categoria_evento = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Premio", x => x.id);
                    table.ForeignKey(
                        name: "FK_Premio_CategoriaEvento_id_categoria_evento",
                        column: x => x.id_categoria_evento,
                        principalTable: "CategoriaEvento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PremioVotacion",
                columns: table => new
                {
                    id_premio = table.Column<Guid>(type: "uuid", nullable: false),
                    id_votacion = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremioVotacion", x => new { x.id_premio, x.id_votacion });
                    table.ForeignKey(
                        name: "FK_PremioVotacion_Premio_id_premio",
                        column: x => x.id_premio,
                        principalTable: "Premio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PremioVotacion_Votacion_id_votacion",
                        column: x => x.id_votacion,
                        principalTable: "Votacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Premio_id_categoria_evento_nombre",
                table: "Premio",
                columns: new[] { "id_categoria_evento", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PremioVotacion_id_votacion",
                table: "PremioVotacion",
                column: "id_votacion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PremioVotacion");

            migrationBuilder.DropTable(
                name: "Premio");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Voto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaId",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "Fecha_voto",
                table: "Voto",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "Id_categoria",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id_proyecto",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id_votacion",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id_votante",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Puntuacion",
                table: "Voto",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VotacionId",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DimensionesDeValorsId",
                table: "Votacione",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaFin",
                table: "Votacione",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaInicio",
                table: "Votacione",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "PermiteAutoVoto",
                table: "Votacione",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ResultadosId",
                table: "Votacione",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "id_evento",
                table: "Votacione",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Voto_CategoriaId",
                table: "Voto",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Voto_UsuarioId",
                table: "Voto",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Voto_VotacionId",
                table: "Voto",
                column: "VotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Votacione_DimensionesDeValorsId",
                table: "Votacione",
                column: "DimensionesDeValorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Votacione_ResultadosId",
                table: "Votacione",
                column: "ResultadosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votacione_DimensionesDeValor_DimensionesDeValorsId",
                table: "Votacione",
                column: "DimensionesDeValorsId",
                principalTable: "DimensionesDeValor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votacione_Resultado_ResultadosId",
                table: "Votacione",
                column: "ResultadosId",
                principalTable: "Resultado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voto_Categoria_CategoriaId",
                table: "Voto",
                column: "CategoriaId",
                principalTable: "Categoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voto_Usuario_UsuarioId",
                table: "Voto",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voto_Votacione_VotacionId",
                table: "Voto",
                column: "VotacionId",
                principalTable: "Votacione",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votacione_DimensionesDeValor_DimensionesDeValorsId",
                table: "Votacione");

            migrationBuilder.DropForeignKey(
                name: "FK_Votacione_Resultado_ResultadosId",
                table: "Votacione");

            migrationBuilder.DropForeignKey(
                name: "FK_Voto_Categoria_CategoriaId",
                table: "Voto");

            migrationBuilder.DropForeignKey(
                name: "FK_Voto_Usuario_UsuarioId",
                table: "Voto");

            migrationBuilder.DropForeignKey(
                name: "FK_Voto_Votacione_VotacionId",
                table: "Voto");

            migrationBuilder.DropIndex(
                name: "IX_Voto_CategoriaId",
                table: "Voto");

            migrationBuilder.DropIndex(
                name: "IX_Voto_UsuarioId",
                table: "Voto");

            migrationBuilder.DropIndex(
                name: "IX_Voto_VotacionId",
                table: "Voto");

            migrationBuilder.DropIndex(
                name: "IX_Votacione_DimensionesDeValorsId",
                table: "Votacione");

            migrationBuilder.DropIndex(
                name: "IX_Votacione_ResultadosId",
                table: "Votacione");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "Fecha_voto",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "Id_categoria",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "Id_proyecto",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "Id_votacion",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "Id_votante",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "Puntuacion",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "VotacionId",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "DimensionesDeValorsId",
                table: "Votacione");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "Votacione");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "Votacione");

            migrationBuilder.DropColumn(
                name: "PermiteAutoVoto",
                table: "Votacione");

            migrationBuilder.DropColumn(
                name: "ResultadosId",
                table: "Votacione");

            migrationBuilder.DropColumn(
                name: "id_evento",
                table: "Votacione");
        }
    }
}

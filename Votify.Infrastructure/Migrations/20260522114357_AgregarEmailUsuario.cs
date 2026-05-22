using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEmailUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Evento_Id_evento",
                table: "Proyecto");

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
                name: "IX_Proyecto_Id_evento",
                table: "Proyecto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votacione",
                table: "Votacione");

            migrationBuilder.DropIndex(
                name: "IX_Votacione_DimensionesDeValorsId",
                table: "Votacione");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
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

            migrationBuilder.RenameTable(
                name: "Votacione",
                newName: "Votacion");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Rol");

            migrationBuilder.RenameColumn(
                name: "id_evento",
                table: "Votacion",
                newName: "Id_evento");

            migrationBuilder.RenameIndex(
                name: "IX_Votacione_ResultadosId",
                table: "Votacion",
                newName: "IX_Votacion_ResultadosId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id_categoria",
                table: "Voto",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha_voto",
                table: "Voto",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Comentario",
                table: "Voto",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuario",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id_rol",
                table: "Usuario",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id_evento",
                table: "Proyecto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id_creador",
                table: "Proyecto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "id_categoria_evento",
                table: "Proyecto",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInicio",
                table: "Evento",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "Evento",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Evento",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "DimensionesDeValor",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "Id_votacion",
                table: "DimensionesDeValor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "DimensionesDeValor",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Peso",
                table: "DimensionesDeValor",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "id_categoria_evento",
                table: "DimensionesDeValor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "ResultadosId",
                table: "Votacion",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInicio",
                table: "Votacion",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "Votacion",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<bool>(
                name: "ComentariosObligatorios",
                table: "Votacion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EstadoVotacion",
                table: "Votacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumProy",
                table: "Votacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteComentarios",
                table: "Votacion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TipoVotacion",
                table: "Votacion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "VerRes",
                table: "Votacion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "id_categoria_evento",
                table: "Votacion",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Rol",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votacion",
                table: "Votacion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rol",
                table: "Rol",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategoriaEvento",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_evento = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaEvento", x => x.id);
                    table.ForeignKey(
                        name: "FK_CategoriaEvento_Evento_id_evento",
                        column: x => x.id_evento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoUsuarioRol",
                columns: table => new
                {
                    id_evento = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    id_rol = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoUsuarioRol", x => new { x.id_evento, x.id_usuario, x.id_rol });
                    table.ForeignKey(
                        name: "FK_EventoUsuarioRol_Evento_id_evento",
                        column: x => x.id_evento,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoUsuarioRol_Rol_id_rol",
                        column: x => x.id_rol,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoUsuarioRol_Usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ProyectoId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Material_Proyecto_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoParticipantes",
                columns: table => new
                {
                    Id_proyecto = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_usuario = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoParticipantes", x => new { x.Id_proyecto, x.Id_usuario });
                    table.ForeignKey(
                        name: "FK_ProyectoParticipantes_Proyecto_Id_proyecto",
                        column: x => x.Id_proyecto,
                        principalTable: "Proyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyectoParticipantes_Usuario_Id_usuario",
                        column: x => x.Id_usuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VotoCriterio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_voto = table.Column<Guid>(type: "uuid", nullable: false),
                    Id_criterio = table.Column<Guid>(type: "uuid", nullable: false),
                    Nota = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotoCriterio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotoCriterio_DimensionesDeValor_Id_criterio",
                        column: x => x.Id_criterio,
                        principalTable: "DimensionesDeValor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotoCriterio_Voto_Id_voto",
                        column: x => x.Id_voto,
                        principalTable: "Voto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Premio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_categoria_evento = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    imagen_url = table.Column<string>(type: "text", nullable: true),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    puesto = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_Voto_Id_categoria",
                table: "Voto",
                column: "Id_categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Voto_Id_votacion",
                table: "Voto",
                column: "Id_votacion");

            migrationBuilder.CreateIndex(
                name: "IX_Voto_Id_votante",
                table: "Voto",
                column: "Id_votante");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Id_rol",
                table: "Usuario",
                column: "Id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_id_categoria_evento",
                table: "Proyecto",
                column: "id_categoria_evento");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_Id_evento",
                table: "Proyecto",
                column: "Id_evento");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionesDeValor_id_categoria_evento",
                table: "DimensionesDeValor",
                column: "id_categoria_evento");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionesDeValor_Id_votacion",
                table: "DimensionesDeValor",
                column: "Id_votacion");

            migrationBuilder.CreateIndex(
                name: "IX_Votacion_id_categoria_evento",
                table: "Votacion",
                column: "id_categoria_evento");

            migrationBuilder.CreateIndex(
                name: "IX_Votacion_Id_evento",
                table: "Votacion",
                column: "Id_evento");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaEvento_id_evento",
                table: "CategoriaEvento",
                column: "id_evento");

            migrationBuilder.CreateIndex(
                name: "IX_EventoUsuarioRol_id_rol",
                table: "EventoUsuarioRol",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_EventoUsuarioRol_id_usuario",
                table: "EventoUsuarioRol",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Material_ProyectoId",
                table: "Material",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Premio_id_categoria_evento_nombre",
                table: "Premio",
                columns: new[] { "id_categoria_evento", "nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PremioVotacion_id_votacion",
                table: "PremioVotacion",
                column: "id_votacion");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoParticipantes_Id_usuario",
                table: "ProyectoParticipantes",
                column: "Id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_VotoCriterio_Id_criterio",
                table: "VotoCriterio",
                column: "Id_criterio");

            migrationBuilder.CreateIndex(
                name: "IX_VotoCriterio_Id_voto_Id_criterio",
                table: "VotoCriterio",
                columns: new[] { "Id_voto", "Id_criterio" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionesDeValor_CategoriaEvento_id_categoria_evento",
                table: "DimensionesDeValor",
                column: "id_categoria_evento",
                principalTable: "CategoriaEvento",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionesDeValor_Votacion_Id_votacion",
                table: "DimensionesDeValor",
                column: "Id_votacion",
                principalTable: "Votacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_CategoriaEvento_id_categoria_evento",
                table: "Proyecto",
                column: "id_categoria_evento",
                principalTable: "CategoriaEvento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_Evento_Id_evento",
                table: "Proyecto",
                column: "Id_evento",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Rol_Id_rol",
                table: "Usuario",
                column: "Id_rol",
                principalTable: "Rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votacion_CategoriaEvento_id_categoria_evento",
                table: "Votacion",
                column: "id_categoria_evento",
                principalTable: "CategoriaEvento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votacion_Evento_Id_evento",
                table: "Votacion",
                column: "Id_evento",
                principalTable: "Evento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votacion_Resultado_ResultadosId",
                table: "Votacion",
                column: "ResultadosId",
                principalTable: "Resultado",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Voto_CategoriaEvento_Id_categoria",
                table: "Voto",
                column: "Id_categoria",
                principalTable: "CategoriaEvento",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Voto_Usuario_Id_votante",
                table: "Voto",
                column: "Id_votante",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voto_Votacion_Id_votacion",
                table: "Voto",
                column: "Id_votacion",
                principalTable: "Votacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionesDeValor_CategoriaEvento_id_categoria_evento",
                table: "DimensionesDeValor");

            migrationBuilder.DropForeignKey(
                name: "FK_DimensionesDeValor_Votacion_Id_votacion",
                table: "DimensionesDeValor");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_CategoriaEvento_id_categoria_evento",
                table: "Proyecto");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Evento_Id_evento",
                table: "Proyecto");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Rol_Id_rol",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_Votacion_CategoriaEvento_id_categoria_evento",
                table: "Votacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Votacion_Evento_Id_evento",
                table: "Votacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Votacion_Resultado_ResultadosId",
                table: "Votacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Voto_CategoriaEvento_Id_categoria",
                table: "Voto");

            migrationBuilder.DropForeignKey(
                name: "FK_Voto_Usuario_Id_votante",
                table: "Voto");

            migrationBuilder.DropForeignKey(
                name: "FK_Voto_Votacion_Id_votacion",
                table: "Voto");

            migrationBuilder.DropTable(
                name: "EventoUsuarioRol");

            migrationBuilder.DropTable(
                name: "Material");

            migrationBuilder.DropTable(
                name: "PremioVotacion");

            migrationBuilder.DropTable(
                name: "ProyectoParticipantes");

            migrationBuilder.DropTable(
                name: "VotoCriterio");

            migrationBuilder.DropTable(
                name: "Premio");

            migrationBuilder.DropTable(
                name: "CategoriaEvento");

            migrationBuilder.DropIndex(
                name: "IX_Voto_Id_categoria",
                table: "Voto");

            migrationBuilder.DropIndex(
                name: "IX_Voto_Id_votacion",
                table: "Voto");

            migrationBuilder.DropIndex(
                name: "IX_Voto_Id_votante",
                table: "Voto");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_Id_rol",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_id_categoria_evento",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_Proyecto_Id_evento",
                table: "Proyecto");

            migrationBuilder.DropIndex(
                name: "IX_DimensionesDeValor_id_categoria_evento",
                table: "DimensionesDeValor");

            migrationBuilder.DropIndex(
                name: "IX_DimensionesDeValor_Id_votacion",
                table: "DimensionesDeValor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Votacion",
                table: "Votacion");

            migrationBuilder.DropIndex(
                name: "IX_Votacion_id_categoria_evento",
                table: "Votacion");

            migrationBuilder.DropIndex(
                name: "IX_Votacion_Id_evento",
                table: "Votacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rol",
                table: "Rol");

            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "Voto");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Id_rol",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Id_creador",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "id_categoria_evento",
                table: "Proyecto");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "DimensionesDeValor");

            migrationBuilder.DropColumn(
                name: "Id_votacion",
                table: "DimensionesDeValor");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "DimensionesDeValor");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "DimensionesDeValor");

            migrationBuilder.DropColumn(
                name: "id_categoria_evento",
                table: "DimensionesDeValor");

            migrationBuilder.DropColumn(
                name: "ComentariosObligatorios",
                table: "Votacion");

            migrationBuilder.DropColumn(
                name: "EstadoVotacion",
                table: "Votacion");

            migrationBuilder.DropColumn(
                name: "NumProy",
                table: "Votacion");

            migrationBuilder.DropColumn(
                name: "PermiteComentarios",
                table: "Votacion");

            migrationBuilder.DropColumn(
                name: "TipoVotacion",
                table: "Votacion");

            migrationBuilder.DropColumn(
                name: "VerRes",
                table: "Votacion");

            migrationBuilder.DropColumn(
                name: "id_categoria_evento",
                table: "Votacion");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Rol");

            migrationBuilder.RenameTable(
                name: "Votacion",
                newName: "Votacione");

            migrationBuilder.RenameTable(
                name: "Rol",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "Id_evento",
                table: "Votacione",
                newName: "id_evento");

            migrationBuilder.RenameIndex(
                name: "IX_Votacion_ResultadosId",
                table: "Votacione",
                newName: "IX_Votacione_ResultadosId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id_categoria",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Fecha_voto",
                table: "Voto",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaId",
                table: "Voto",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AlterColumn<Guid>(
                name: "Id_evento",
                table: "Proyecto",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaInicio",
                table: "Evento",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaFin",
                table: "Evento",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResultadosId",
                table: "Votacione",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaInicio",
                table: "Votacione",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaFin",
                table: "Votacione",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "DimensionesDeValorsId",
                table: "Votacione",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Votacione",
                table: "Votacione",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

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
                name: "IX_Proyecto_Id_evento",
                table: "Proyecto",
                column: "Id_evento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votacione_DimensionesDeValorsId",
                table: "Votacione",
                column: "DimensionesDeValorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_Evento_Id_evento",
                table: "Proyecto",
                column: "Id_evento",
                principalTable: "Evento",
                principalColumn: "Id");

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
    }
}

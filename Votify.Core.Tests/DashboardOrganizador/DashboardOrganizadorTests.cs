using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.API.Controllers;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.DashboardOrganizador
{
    [TestClass]
    public class DashboardOrganizadorTests
    {
        private AppDbContext context;
        private EventoService eventoService;
        private VotacionService votacionService;

        private Guid idOrganizador;
        private Guid idOtroOrganizador;
        private Guid idEventoActivo;
        private Guid idEventoArchivado;
        private Guid idEventoAjeno;
        private Guid idVotacionActiva;
        private Guid idVotacionArchivada;
        private Guid idVotacionAjena;

        [TestInitialize]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);

            var eventoRepository = new EventoRepository(context);
            var votacionRepository = new VotacionRepository(context);

            eventoService = new EventoService(eventoRepository);
            votacionService = new VotacionService(
                votacionRepository,
                new VotoRepository(context),
                new ProyectoRepository(context),
                new DimensionRepository(context),
                new UsuarioRepository(context),
                new EventoUsuarioRolRepository(context)
            );

            await CrearDatosDashboardOrganizador();
        }

        [TestMethod]
        public async Task EstadisticasOrganizador_CuentaSoloDatosDeEventosDelOrganizador()
        {
            var controller = new EventosController(
                eventoService,
                votacionService,
                context,
                new AzureStorageFalso()
            );

            var resultado = await controller.EstadisticasOrganizador(idOrganizador);

            Assert.AreEqual(1, resultado.EventosActivos);
            Assert.AreEqual(3, resultado.ProyectosTotales);
            Assert.AreEqual(2, resultado.Participantes);
            Assert.AreEqual(3, resultado.VotosEmitidos);
        }

        [TestMethod]
        public async Task ObtenerEventosOrganizador_RetornaSoloEventosDondeUsuarioEsOrganizador()
        {
            var eventos = await eventoService.ObtenerEventosOrganizador(idOrganizador);

            Assert.AreEqual(2, eventos.Count);
            Assert.IsTrue(eventos.Any(e => e.Id == idEventoActivo));
            Assert.IsTrue(eventos.Any(e => e.Id == idEventoArchivado));
            Assert.IsFalse(eventos.Any(e => e.Id == idEventoAjeno));
            Assert.IsTrue(eventos.All(e => e.Categoria != null));
        }

        [TestMethod]
        public async Task NumeroVotosPorEvento_CuentaVotosDeProyectosDelEvento()
        {
            var numeroVotos = await eventoService.NumeroVotosPorEvento(idEventoActivo);

            Assert.AreEqual(2, numeroVotos);
        }

        [TestMethod]
        public async Task ObtenerVotacionesEventos_RetornaVotacionesDeLosEventosIndicados()
        {
            var votaciones = await votacionService.ObtenerVotacionesEventos(
                new List<Guid> { idEventoActivo, idEventoArchivado }
            );

            Assert.AreEqual(2, votaciones.Count);
            Assert.IsTrue(votaciones.Any(v => v.Id == idVotacionActiva));
            Assert.IsTrue(votaciones.Any(v => v.Id == idVotacionArchivada));
            Assert.IsFalse(votaciones.Any(v => v.Id == idVotacionAjena));
        }

        [TestMethod]
        public async Task ObtenerVotosVotacion_RetornaSoloVotosDeLaVotacionIndicada()
        {
            var votos = await votacionService.ObtenerVotosVotacion(idVotacionActiva);

            Assert.AreEqual(2, votos.Count);
            Assert.IsTrue(votos.All(v => v.Id_votacion == idVotacionActiva));
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        private async Task CrearDatosDashboardOrganizador()
        {
            idOrganizador = Guid.NewGuid();
            idOtroOrganizador = Guid.NewGuid();
            idEventoActivo = Guid.NewGuid();
            idEventoArchivado = Guid.NewGuid();
            idEventoAjeno = Guid.NewGuid();
            idVotacionActiva = Guid.NewGuid();
            idVotacionArchivada = Guid.NewGuid();
            idVotacionAjena = Guid.NewGuid();

            var idParticipanteCompartido = Guid.NewGuid();
            var idParticipanteSoloActivo = Guid.NewGuid();
            var idParticipanteAjeno = Guid.NewGuid();

            var rolOrganizador = new Rol { Id = Guid.NewGuid(), Nombre = "Organizador" };
            var rolParticipante = new Rol { Id = Guid.NewGuid(), Nombre = "Participante" };
            var categoria = new Categoria { Id = Guid.NewGuid(), Nombre = "Categoria test" };

            context.Role.AddRange(rolOrganizador, rolParticipante);
            context.Categoria.Add(categoria);

            context.Usuario.AddRange(
                CrearUsuario(idOrganizador, "organizador", rolOrganizador),
                CrearUsuario(idOtroOrganizador, "otroOrganizador", rolOrganizador),
                CrearUsuario(idParticipanteCompartido, "participanteCompartido", rolParticipante),
                CrearUsuario(idParticipanteSoloActivo, "participanteActivo", rolParticipante),
                CrearUsuario(idParticipanteAjeno, "participanteAjeno", rolParticipante)
            );

            context.Evento.AddRange(
                CrearEvento(idEventoActivo, "Evento activo", categoria, archivado: false),
                CrearEvento(idEventoArchivado, "Evento archivado", categoria, archivado: true),
                CrearEvento(idEventoAjeno, "Evento ajeno", categoria, archivado: false)
            );

            context.EventoUsuarioRol.AddRange(
                CrearRelacion(idEventoActivo, idOrganizador, rolOrganizador),
                CrearRelacion(idEventoArchivado, idOrganizador, rolOrganizador),
                CrearRelacion(idEventoAjeno, idOtroOrganizador, rolOrganizador),
                CrearRelacion(idEventoActivo, idParticipanteCompartido, rolParticipante),
                CrearRelacion(idEventoArchivado, idParticipanteCompartido, rolParticipante),
                CrearRelacion(idEventoActivo, idParticipanteSoloActivo, rolParticipante),
                CrearRelacion(idEventoAjeno, idParticipanteAjeno, rolParticipante)
            );

            var idProyectoActivo1 = Guid.NewGuid();
            var idProyectoActivo2 = Guid.NewGuid();
            var idProyectoArchivado = Guid.NewGuid();
            var idProyectoAjeno = Guid.NewGuid();

            context.Proyecto.AddRange(
                CrearProyecto(idProyectoActivo1, idEventoActivo, "Proyecto activo 1"),
                CrearProyecto(idProyectoActivo2, idEventoActivo, "Proyecto activo 2"),
                CrearProyecto(idProyectoArchivado, idEventoArchivado, "Proyecto archivado"),
                CrearProyecto(idProyectoAjeno, idEventoAjeno, "Proyecto ajeno")
            );

            context.Votacion.AddRange(
                CrearVotacion(idVotacionActiva, idEventoActivo),
                CrearVotacion(idVotacionArchivada, idEventoArchivado),
                CrearVotacion(idVotacionAjena, idEventoAjeno)
            );

            context.Voto.AddRange(
                CrearVoto(idVotacionActiva, idProyectoActivo1),
                CrearVoto(idVotacionActiva, idProyectoActivo2),
                CrearVoto(idVotacionArchivada, idProyectoArchivado),
                CrearVoto(idVotacionAjena, idProyectoAjeno),
                CrearVoto(idVotacionAjena, idProyectoAjeno)
            );

            await context.SaveChangesAsync();
        }

        private static Usuario CrearUsuario(Guid id, string nombre, Rol rol)
        {
            return new Usuario
            {
                Id = id,
                Nombre = nombre,
                Password = "1234",
                Id_rol = rol.Id,
                Rol = rol
            };
        }

        private static Evento CrearEvento(Guid id, string nombre, Categoria categoria, bool archivado)
        {
            return new EventoHackathon
            {
                Id = id,
                Nombre = nombre,
                Descripcion = "Evento de prueba",
                FechaInicio = DateTime.UtcNow.AddDays(-1),
                FechaFin = DateTime.UtcNow.AddDays(7),
                Id_categoria = categoria.Id,
                Categoria = categoria,
                Archivado = archivado
            };
        }

        private static EventoUsuarioRol CrearRelacion(Guid idEvento, Guid idUsuario, Rol rol)
        {
            return new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idUsuario,
                RolId = rol.Id,
                Rol = rol
            };
        }

        private static Proyecto CrearProyecto(Guid id, Guid idEvento, string titulo)
        {
            return new Proyecto
            {
                Id = id,
                Titulo = titulo,
                Descripcion = "Proyecto de prueba",
                Id_evento = idEvento
            };
        }

        private static VotacionPublico CrearVotacion(Guid id, Guid idEvento)
        {
            return new VotacionPublico
            {
                Id = id,
                Id_evento = idEvento,
                FechaInicio = DateTime.UtcNow.AddDays(-1),
                FechaFin = DateTime.UtcNow.AddDays(1),
                PermiteAutoVoto = true,
                NumProy = 2,
                EstadoVotacion = EstadoVotacion.Abierta
            };
        }

        private static Voto CrearVoto(Guid idVotacion, Guid idProyecto)
        {
            return new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_proyecto = idProyecto,
                Id_votante = Guid.NewGuid(),
                Puntuacion = 1,
                Fecha_voto = DateTime.UtcNow
            };
        }

        private sealed class AzureStorageFalso : IAzureStorage
        {
            public Task<string> SubirArchivoAsync(Stream contenidoArchivo, string nombreArchivo)
            {
                return Task.FromResult($"https://storage.test/{nombreArchivo}");
            }

            public Task EliminarArchivoAsync(string url)
            {
                return Task.CompletedTask;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Factories;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Votacion
{
    [TestClass]
    public class VotacionServiceTests
    {
        private VotacionService votacionService;
        private AppDbContext context;
        private ConcreteEventFactory factory;

        private Guid idEvento;
        private Guid idVotacion;
        private Guid idVotante;
        private Guid idProyecto1;
        private Guid idProyecto2;
        private Guid idProyecto3;
        private Guid idProyectoOtroEvento;

        private List<Guid> JuradosVacios => new List<Guid>();
        private List<CrearCriterioDTO> CriteriosVacios => new List<CrearCriterioDTO>();

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);
            factory = new ConcreteEventFactory();

            var votacionRepository = new VotacionRepository(context);
            var votoRepository = new VotoRepository(context);
            var proyectoRepository = new ProyectoRepository(context);
            var dimensionRepository = new DimensionRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var eventoUsuarioRolRepository = new EventoUsuarioRolRepository(context);

            votacionService = new VotacionService(
                votacionRepository,
                votoRepository,
                proyectoRepository,
                dimensionRepository,
                usuarioRepository,
                eventoUsuarioRolRepository
            );

            idVotacion = Guid.NewGuid();
            idVotante = Guid.NewGuid();

            idProyecto1 = Guid.NewGuid();
            idProyecto2 = Guid.NewGuid();
            idProyecto3 = Guid.NewGuid();
            idProyectoOtroEvento = Guid.NewGuid();

            var hoy = DateTime.UtcNow;
            var categoriaId = Guid.NewGuid();

            var categoria = new Categoria
            {
                Id = categoriaId,
                Nombre = "Categoria test"
            };

            var eventoPrincipal = factory.CreateEvento(
                "Hackathon",
                "Evento test",
                "Descripcion test",
                hoy.AddDays(-1),
                hoy.AddDays(1),
                categoriaId
            );

            idEvento = eventoPrincipal.Id;

            var otroEvento = factory.CreateEvento(
                "Hackathon",
                "Otro evento",
                "Descripcion otro evento",
                hoy.AddDays(-1),
                hoy.AddDays(1),
                categoriaId
            );

            var otroEvento2 = factory.CreateEvento(
                "Concurso",
                "Otro evento 2",
                "Descripcion otro evento 2",
                hoy.AddDays(-1),
                hoy.AddDays(1),
                categoriaId
            );

            var idOtroEvento = otroEvento2.Id;

            var rolPublico = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = "Publico"
            };

            var usuario = new Usuario
            {
                Id = idVotante,
                Nombre = "usuarioTest",
                Password = "1234",
                Id_rol = rolPublico.Id,
                Rol = rolPublico
            };

            var eventoUsuarioRol = new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idVotante,
                RolId = rolPublico.Id,
                Rol = rolPublico
            };

            var votacion = new VotacionPublico
            {
                Id = idVotacion,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1),
                PermiteAutoVoto = false,
                EstadoVotacion = EstadoVotacion.Abierta
            };

            context.Categoria.Add(categoria);
            context.Role.Add(rolPublico);
            context.Usuario.Add(usuario);
            context.EventoUsuarioRol.Add(eventoUsuarioRol);
            context.Evento.AddRange(eventoPrincipal, otroEvento, otroEvento2);
            context.Votacion.Add(votacion);

            context.Proyecto.AddRange(
                new Proyecto
                {
                    Id = idProyecto1,
                    Titulo = "Proyecto 1",
                    Descripcion = "Descripcion proyecto 1",
                    Id_evento = idEvento
                },
                new Proyecto
                {
                    Id = idProyecto2,
                    Titulo = "Proyecto 2",
                    Descripcion = "Descripcion proyecto 2",
                    Id_evento = idEvento
                },
                new Proyecto
                {
                    Id = idProyecto3,
                    Titulo = "Proyecto 3",
                    Descripcion = "Descripcion proyecto 3",
                    Id_evento = idEvento
                },
                new Proyecto
                {
                    Id = idProyectoOtroEvento,
                    Titulo = "Proyecto otro evento",
                    Descripcion = "Descripcion proyecto otro evento",
                    Id_evento = idOtroEvento
                }
            );

            context.DimensionesDeValor.AddRange(
                new DimensionesDeValor
                {
                    Id = Guid.NewGuid(),
                    Id_votacion = idVotacion,
                    Nombre = "Criterio 1",
                    Descripcion = "Descripción",
                    Peso = 50
                },
                new DimensionesDeValor
                {
                    Id = Guid.NewGuid(),
                    Id_votacion = idVotacion,
                    Nombre = "Criterio 2",
                    Descripcion = "Descripción",
                    Peso = 50
                }
            );

            context.SaveChanges();
        }

        private EmitirVotoDTO CrearDtoVoto(Guid votanteId)
        {
            return new EmitirVotoDTO
            {
                IdVotante = votanteId,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyecto1 },
                    new VotoDetalleDTO { IdProyecto = idProyecto2 },
                    new VotoDetalleDTO { IdProyecto = idProyecto3 }
                }
            };
        }

        private async Task CrearVotacionAsync(
            DateTime inicio,
            DateTime fin,
            bool permiteAutoVoto = false,
            EstadoVotacion estado = EstadoVotacion.Abierta)
        {
            await votacionService.CrearVotacion(
                idEvento,
                inicio,
                fin,
                permiteAutoVoto,
                TipoVotacion.Publico,
                3,
                JuradosVacios,
                estado,
                CriteriosVacios
            );
        }

        [TestMethod]
        public async Task RegistrarVotoDtoNull_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, null)
            );

            Assert.AreEqual("El DTO del voto es nulo.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoListaProyectosNull_LanzaExcepcion()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdEvento = idEvento,
                Votos = null
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, dto)
            );

            Assert.AreEqual("Debes votar exactamente 3 proyectos.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoMenosDe3Proyectos_LanzaExcepcion()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyecto1 },
                    new VotoDetalleDTO { IdProyecto = idProyecto2 }
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, dto)
            );

            Assert.AreEqual("Debes votar exactamente 3 proyectos.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoMasDe3Proyectos_LanzaExcepcion()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyecto1 },
                    new VotoDetalleDTO { IdProyecto = idProyecto2 },
                    new VotoDetalleDTO { IdProyecto = idProyecto3 },
                    new VotoDetalleDTO { IdProyecto = Guid.NewGuid() }
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, dto)
            );

            Assert.AreEqual("No puedes votar más proyectos que el máximo permitido.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoProyectosRepetidos_LanzaExcepcion()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyecto1 },
                    new VotoDetalleDTO { IdProyecto = idProyecto1 },
                    new VotoDetalleDTO { IdProyecto = idProyecto2 }
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, dto)
            );

            Assert.AreEqual("No puedes votar dos veces al mismo proyecto.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoVotacionNoExiste_LanzaExcepcion()
        {
            var dto = CrearDtoVoto(idVotante);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(Guid.NewGuid(), dto)
            );

            Assert.AreEqual("La votación no existe.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoFueraDeFecha_LanzaExcepcion()
        {
            var votacionFueraFechaId = Guid.NewGuid();
            var hoy = DateTime.UtcNow;

            context.Votacion.Add(new VotacionPublico
            {
                Id = votacionFueraFechaId,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(2),
                FechaFin = hoy.AddDays(5),
                PermiteAutoVoto = false,
                EstadoVotacion = EstadoVotacion.Abierta
            });

            await context.SaveChangesAsync();

            var dto = CrearDtoVoto(Guid.NewGuid());

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(votacionFueraFechaId, dto)
            );

            Assert.AreEqual("La votación no está activa.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoUsuarioYaVoto_LanzaExcepcion()
        {
            context.Voto.Add(new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_proyecto = idProyecto1,
                Id_votante = idVotante,
                Puntuacion = 1,
                Fecha_voto = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var dto = CrearDtoVoto(idVotante);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, dto)
            );

            Assert.AreEqual("Ya has votado en esta votación.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoProyectoNoExiste_LanzaExcepcion()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = Guid.NewGuid(),
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyecto1 },
                    new VotoDetalleDTO { IdProyecto = idProyecto2 },
                    new VotoDetalleDTO { IdProyecto = Guid.NewGuid() }
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, dto)
            );

            Assert.AreEqual("No se han encontrado los 3 proyectos que debías votar.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoProyectoDeOtroEvento_LanzaExcepcion()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = Guid.NewGuid(),
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyecto1 },
                    new VotoDetalleDTO { IdProyecto = idProyecto2 },
                    new VotoDetalleDTO { IdProyecto = idProyectoOtroEvento }
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacion, dto)
            );

            Assert.AreEqual("Algún proyecto no pertenece al evento.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVotoValid_ReturnsTrue_AndGuarda3Votos()
        {
            var dto = CrearDtoVoto(idVotante);

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsTrue(resultado);

            var votosGuardados = await context.Voto
                .Where(v => v.Id_votacion == idVotacion && v.Id_votante == idVotante)
                .ToListAsync();

            Assert.AreEqual(3, votosGuardados.Count);
        }

        [TestMethod]
        public async Task RegistrarVoto_GuardaFechaCorrecta()
        {
            var dto = CrearDtoVoto(idVotante);

            await votacionService.RegistrarVotoAsync(idVotacion, dto);

            var voto = await context.Voto.FirstOrDefaultAsync(v => v.Id_votante == idVotante);

            Assert.IsNotNull(voto);
            Assert.AreEqual(DateTime.UtcNow.Date, voto.Fecha_voto.Date);
        }

        [TestMethod]
        public async Task CrearVotacion_ValidData_CreaVotacionCorrectamente()
        {
            await CrearVotacionAsync(
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(10),
                permiteAutoVoto: true
            );

            var votacionCreada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id_evento == idEvento && v.Id != idVotacion);

            Assert.IsNotNull(votacionCreada);
            Assert.AreEqual(idEvento, votacionCreada.Id_evento);
            Assert.IsTrue(votacionCreada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task CrearVotacion_EventoVacio_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.CrearVotacion(
                    Guid.Empty,
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(10),
                    true,
                    TipoVotacion.Publico,
                    3,
                    JuradosVacios,
                    EstadoVotacion.Abierta,
                    CriteriosVacios
                )
            );

            Assert.AreEqual("El evento es obligatorio", exception.Message);
        }

        [TestMethod]
        public async Task CrearVotacion_FechaFinAnteriorAInicio_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => CrearVotacionAsync(
                    DateTime.UtcNow.AddDays(10),
                    DateTime.UtcNow.AddDays(1),
                    permiteAutoVoto: true
                )
            );

            Assert.AreEqual("La fecha de fin no puede ser anterior a la fecha de inicio", exception.Message);
        }

        [TestMethod]
        public async Task CrearVotacion_FechasIguales_NoLanzaExcepcion()
        {
            var fecha = DateTime.UtcNow.AddDays(5);

            await CrearVotacionAsync(fecha, fecha);

            var votacionCreada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id_evento == idEvento && v.Id != idVotacion);

            Assert.IsNotNull(votacionCreada);
        }

        [TestMethod]
        public async Task CrearVotacion_PermiteAutoVotoFalse_SeGuardaCorrectamente()
        {
            await CrearVotacionAsync(
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(10),
                permiteAutoVoto: false
            );

            var votacionCreada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id_evento == idEvento && v.Id != idVotacion);

            Assert.IsNotNull(votacionCreada);
            Assert.IsFalse(votacionCreada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task CrearVotacion_GeneraIdUnico()
        {
            var inicio = DateTime.UtcNow.AddDays(1);
            var fin = DateTime.UtcNow.AddDays(10);

            await CrearVotacionAsync(inicio, fin, true);
            await CrearVotacionAsync(inicio, fin, false);

            var votaciones = await context.Votacion
                .Where(v => v.Id_evento == idEvento && v.Id != idVotacion)
                .ToListAsync();

            Assert.AreEqual(2, votaciones.Count);
            Assert.AreNotEqual(votaciones[0].Id, votaciones[1].Id);
        }

        [TestMethod]
        public async Task EditarVotacion_ValidData_ActualizaCorrectamente()
        {
            var nuevoEvento = Guid.NewGuid();

            context.Evento.Add(new EventoHackathon
            {
                Id = nuevoEvento,
                Nombre = "Nuevo Evento",
                Descripcion = "Descripcion nuevo evento",
                FechaInicio = DateTime.UtcNow.AddDays(-1),
                FechaFin = DateTime.UtcNow.AddDays(30),
                Id_categoria = context.Categoria.First().Id
            });

            await context.SaveChangesAsync();

            await votacionService.EditarVotacion(
                idVotacion,
                nuevoEvento,
                DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow.AddDays(15),
                true,
                TipoVotacion.Publico,
                JuradosVacios,
                EstadoVotacion.Abierta,
                CriteriosVacios,
                3
            );

            var votacionEditada = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacionEditada);
            Assert.AreEqual(nuevoEvento, votacionEditada.Id_evento);
            Assert.IsTrue(votacionEditada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task EditarVotacion_VotacionNoExiste_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EditarVotacion(
                    Guid.NewGuid(),
                    idEvento,
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(10),
                    true,
                    TipoVotacion.Publico,
                    JuradosVacios,
                    EstadoVotacion.Abierta,
                    CriteriosVacios,
                    3
                )
            );

            Assert.AreEqual("La votación no existe", exception.Message);
        }

        [TestMethod]
        public async Task EditarVotacion_EventoVacio_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EditarVotacion(
                    idVotacion,
                    Guid.Empty,
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(10),
                    true,
                    TipoVotacion.Publico,
                    JuradosVacios,
                    EstadoVotacion.Abierta,
                    CriteriosVacios,
                    3
                )
            );

            Assert.AreEqual("El evento es obligatorio", exception.Message);
        }

        [TestMethod]
        public async Task EditarVotacion_FechaFinAnteriorAInicio_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EditarVotacion(
                    idVotacion,
                    idEvento,
                    DateTime.UtcNow.AddDays(10),
                    DateTime.UtcNow.AddDays(1),
                    true,
                    TipoVotacion.Publico,
                    JuradosVacios,
                    EstadoVotacion.Abierta,
                    CriteriosVacios,
                    3
                )
            );

            Assert.AreEqual("La fecha de fin no puede ser anterior a la fecha de inicio", exception.Message);
        }

        [TestMethod]
        public async Task EditarVotacion_CambiaPermiteAutoVoto_ActualizaCorrectamente()
        {
            var votacionOriginal = await context.Votacion.FindAsync(idVotacion);
            var permisoOriginal = votacionOriginal.PermiteAutoVoto;

            await votacionService.EditarVotacion(
                idVotacion,
                idEvento,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(10),
                !permisoOriginal,
                TipoVotacion.Publico,
                JuradosVacios,
                EstadoVotacion.Abierta,
                CriteriosVacios,
                3
            );

            var votacionActualizada = await context.Votacion.FindAsync(idVotacion);

            Assert.AreEqual(!permisoOriginal, votacionActualizada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task EditarVotacion_FechasIguales_NoLanzaExcepcion()
        {
            var fecha = new DateTime(
                2026, 5, 8, 9, 54, 15,
                DateTimeKind.Local
            );

            await votacionService.EditarVotacion(
                idVotacion,
                idEvento,
                fecha,
                fecha,
                true,
                TipoVotacion.Publico,
                JuradosVacios,
                EstadoVotacion.Abierta,
                CriteriosVacios,
                3
            );

            var votacionEditada = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacionEditada);
            Assert.AreEqual(fecha.ToUniversalTime(), votacionEditada.FechaInicio);
            Assert.AreEqual(fecha.ToUniversalTime(), votacionEditada.FechaFin);
        }

        [TestMethod]
        public async Task EditarVotacion_ConservaIdOriginal()
        {
            await votacionService.EditarVotacion(
                idVotacion,
                idEvento,
                DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow.AddDays(15),
                true,
                TipoVotacion.Publico,
                JuradosVacios,
                EstadoVotacion.Abierta,
                CriteriosVacios,
                3
            );

            var votacionEditada = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacionEditada);
            Assert.AreEqual(idVotacion, votacionEditada.Id);
        }

        [TestMethod]
        public async Task EliminarVotacion_VotacionExiste_EliminaCorrectamente()
        {
            await votacionService.EliminarVotacion(idVotacion);

            var votacionDespues = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNull(votacionDespues);
        }

        [TestMethod]
        public async Task EliminarVotacion_VotacionNoExiste_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EliminarVotacion(Guid.NewGuid())
            );

            Assert.AreEqual("La votación no existe", exception.Message);
        }

        [TestMethod]
        public async Task EliminarVotacion_NoAfectaOtrasVotaciones()
        {
            var otraVotacionId = Guid.NewGuid();
            var hoy = DateTime.UtcNow;

            context.Votacion.Add(new VotacionPublico
            {
                Id = otraVotacionId,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1),
                PermiteAutoVoto = false,
                EstadoVotacion = EstadoVotacion.Abierta
            });

            await context.SaveChangesAsync();

            await votacionService.EliminarVotacion(idVotacion);

            var otraVotacion = await context.Votacion.FindAsync(otraVotacionId);

            Assert.IsNotNull(otraVotacion);
        }

        [TestMethod]
        public async Task EliminarVotacion_ReduceConteoDeVotaciones()
        {
            var conteoAntes = await context.Votacion.CountAsync();

            await votacionService.EliminarVotacion(idVotacion);

            var conteoDespues = await context.Votacion.CountAsync();

            Assert.AreEqual(conteoAntes - 1, conteoDespues);
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_VotacionExiste_RetornaVotacion()
        {
            var resultado = await votacionService.ObtenerPorIdAsync(idVotacion);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(idVotacion, resultado.Id);
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_VotacionNoExiste_RetornaNull()
        {
            var resultado = await votacionService.ObtenerPorIdAsync(Guid.NewGuid());

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_RetornaDatosCorrectos()
        {
            var resultado = await votacionService.ObtenerPorIdAsync(idVotacion);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(idEvento, resultado.Id_evento);
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_GuidEmpty_RetornaNull()
        {
            var resultado = await votacionService.ObtenerPorIdAsync(Guid.Empty);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task BuscarVotacion_ConEventoExistente_RetornaVotaciones()
        {
            var resultado = await votacionService.BuscarVotacion(idEvento, 0, 0, true);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public async Task BuscarVotacion_ConEventoInexistente_RetornaListaVacia()
        {
            var resultado = await votacionService.BuscarVotacion(Guid.NewGuid(), 0, 0, true);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public async Task BuscarVotacion_FiltroPermiteAutoVoto_FiltraCorrectamente()
        {
            context.Votacion.Add(new VotacionPublico
            {
                Id = Guid.NewGuid(),
                Id_evento = idEvento,
                FechaInicio = DateTime.UtcNow.AddDays(-1),
                FechaFin = DateTime.UtcNow.AddDays(1),
                PermiteAutoVoto = true,
                EstadoVotacion = EstadoVotacion.Abierta
            });

            await context.SaveChangesAsync();

            var resultado = await votacionService.BuscarVotacion(idEvento, 0, 1, false);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.All(v => v.PermiteAutoVoto));
        }

        [TestMethod]
        public async Task BuscarVotacion_VerFinalizadasTrue_IncluyeFinalizadas()
        {
            var votacionFinalizada = Guid.NewGuid();

            context.Votacion.Add(new VotacionPublico
            {
                Id = votacionFinalizada,
                Id_evento = idEvento,
                FechaInicio = DateTime.UtcNow.AddDays(-10),
                FechaFin = DateTime.UtcNow.AddDays(-5),
                PermiteAutoVoto = false,
                EstadoVotacion = EstadoVotacion.Finalizada
            });

            await context.SaveChangesAsync();

            var resultado = await votacionService.BuscarVotacion(idEvento, 0, 0, true);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Any(v => v.Id == votacionFinalizada));
        }

        [TestMethod]
        public async Task BuscarVotacion_VerFinalizadasFalse_ExcluyeFinalizadas()
        {
            var resultado = await votacionService.BuscarVotacion(idEvento, 0, 0, false);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.All(v => v.FechaFin > DateTime.UtcNow));
        }
    }
}
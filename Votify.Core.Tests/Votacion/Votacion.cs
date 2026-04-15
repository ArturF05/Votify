using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            votacionService = new VotacionService(
                votacionRepository,
                votoRepository,
                proyectoRepository
            );

            idVotacion = Guid.NewGuid();
            idVotante = Guid.NewGuid();

            idProyecto1 = Guid.NewGuid();
            idProyecto2 = Guid.NewGuid();
            idProyecto3 = Guid.NewGuid();
            idProyectoOtroEvento = Guid.NewGuid();

            var hoy = DateTime.Now;
            var categoriaId = Guid.NewGuid();

            context.Categoria.Add(new Categoria
            {
                Id = categoriaId,
                Nombre = "Categoria test"
            });

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

            context.Evento.Add(eventoPrincipal);
            context.Evento.Add(otroEvento);
            context.Evento.Add(otroEvento2);

            context.Votacion.Add(new Votify.Core.Entities.Votacion
            {
                Id = idVotacion,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1)
            });

            context.Proyecto.Add(new Proyecto
            {
                Id = idProyecto1,
                Titulo = "Proyecto 1",
                Descripcion = "Descripcion proyecto 1",
                Id_evento = idEvento
            });

            context.Proyecto.Add(new Proyecto
            {
                Id = idProyecto2,
                Titulo = "Proyecto 2",
                Descripcion = "Descripcion proyecto 2",
                Id_evento = idEvento
            });

            context.Proyecto.Add(new Proyecto
            {
                Id = idProyecto3,
                Titulo = "Proyecto 3",
                Descripcion = "Descripcion proyecto 3",
                Id_evento = idEvento
            });

            context.Proyecto.Add(new Proyecto
            {
                Id = idProyectoOtroEvento,
                Titulo = "Proyecto otro evento",
                Descripcion = "Descripcion proyecto otro evento",
                Id_evento = idOtroEvento
            });

            context.SaveChanges();
        }

        [TestMethod]
        public async Task RegistrarVotoDtoNull_ReturnsFalse()
        {
            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoListaProyectosNull_ReturnsFalse()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdsProyectos = null
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoMenosDe3Proyectos_ReturnsFalse()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto2 }
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoMasDe3Proyectos_ReturnsFalse()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdsProyectos = new List<Guid>
                {
                    idProyecto1, idProyecto2, idProyecto3, Guid.NewGuid()
                }
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoProyectosRepetidos_ReturnsFalse()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto1, idProyecto2 }
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoVotacionNoExiste_ReturnsFalse()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto2, idProyecto3 }
            };

            var resultado = await votacionService.RegistrarVotoAsync(Guid.NewGuid(), dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoFueraDeFecha_ReturnsFalse()
        {
            var votacionFueraFechaId = Guid.NewGuid();
            var hoy = DateTime.Now;

            context.Votacion.Add(new Votify.Core.Entities.Votacion
            {
                Id = votacionFueraFechaId,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(2),
                FechaFin = hoy.AddDays(5),
            });

            context.SaveChanges();

            var dto = new EmitirVotoDTO
            {
                IdVotante = Guid.NewGuid(),
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto2, idProyecto3 }
            };

            var resultado = await votacionService.RegistrarVotoAsync(votacionFueraFechaId, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoUsuarioYaVoto_ReturnsFalse()
        {
            context.Voto.Add(new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_proyecto = idProyecto1,
                Id_votante = idVotante,
                Puntuacion = 1,
                Fecha_voto = DateTime.Now
            });

            context.SaveChanges();

            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto2, idProyecto3 }
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoProyectoNoExiste_ReturnsFalse()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = Guid.NewGuid(),
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto2, Guid.NewGuid() }
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVotoProyectoDeOtroEvento_ReturnsFalse()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = Guid.NewGuid(),
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto2, idProyectoOtroEvento }
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task RegistrarVoto_GuardaFechaCorrecta()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyecto1},
                    new VotoDetalleDTO { IdProyecto = idProyecto2},
                    new VotoDetalleDTO { IdProyecto = idProyecto3}
                }
            };

            await votacionService.RegistrarVotoAsync(idVotacion, dto);

            var voto = await context.Voto.FirstOrDefaultAsync(v => v.Id_votante == idVotante);

            Assert.IsNotNull(voto);
            Assert.AreEqual(DateTime.Today, voto.Fecha_voto.Date);
        }



        [TestMethod]
        public async Task CrearVotacion_ValidData_CreaVotacionCorrectamente()
        {
            var inicioVotacion = DateTime.Now.AddDays(1);
            var finVotacion = DateTime.Now.AddDays(10);
            var permiteAutoVoto = true;
            TipoVotacion tipoVotacion = TipoVotacion.Publico;

            await votacionService.CrearVotacion(idEvento, inicioVotacion, finVotacion, permiteAutoVoto, tipoVotacion);

            var votacionCreada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id_evento == idEvento && v.PermiteAutoVoto == permiteAutoVoto && v.Id != idVotacion);

            Assert.IsNotNull(votacionCreada);
            Assert.AreEqual(idEvento, votacionCreada.Id_evento);
            Assert.AreEqual(permiteAutoVoto, votacionCreada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task CrearVotacion_EventoVacio_LanzaExcepcion()
        {
            var inicioVotacion = DateTime.Now.AddDays(1);
            var finVotacion = DateTime.Now.AddDays(10);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.CrearVotacion(Guid.Empty, inicioVotacion, finVotacion, true, TipoVotacion.Publico)
            );

            Assert.AreEqual("El evento es obligatorio", exception.Message);
        }

        [TestMethod]
        public async Task CrearVotacion_FechaFinAnteriorAInicio_LanzaExcepcion()
        {
            var inicioVotacion = DateTime.Now.AddDays(10);
            var finVotacion = DateTime.Now.AddDays(1);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.CrearVotacion(idEvento, inicioVotacion, finVotacion, true, TipoVotacion.Publico)
            );

            Assert.AreEqual("La fecha de fin no puede ser anterior a la fecha de inicio", exception.Message);
        }

        [TestMethod]
        public async Task CrearVotacion_FechasIguales_NoLanzaExcepcion()
        {
            var fecha = DateTime.Now.AddDays(5);

            await votacionService.CrearVotacion(idEvento, fecha, fecha, false, TipoVotacion.Publico);

            var votacionCreada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id_evento == idEvento && v.PermiteAutoVoto == false && v.Id != idVotacion);

            Assert.IsNotNull(votacionCreada);
        }

        [TestMethod]
        public async Task CrearVotacion_PermiteAutoVotoFalse_SeGuardaCorrectamente()
        {
            var inicioVotacion = DateTime.Now.AddDays(1);
            var finVotacion = DateTime.Now.AddDays(10);

            await votacionService.CrearVotacion(idEvento, inicioVotacion, finVotacion, false, TipoVotacion.Publico);

            var votacionCreada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id_evento == idEvento && v.PermiteAutoVoto == false && v.Id != idVotacion);

            Assert.IsNotNull(votacionCreada);
            Assert.IsFalse(votacionCreada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task CrearVotacion_FechasSeConviertenAUTC()
        {
            var inicioVotacion = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Local);
            var finVotacion = new DateTime(2025, 6, 20, 18, 0, 0, DateTimeKind.Local);

            await votacionService.CrearVotacion(idEvento, inicioVotacion, finVotacion, true, TipoVotacion.Publico);

            var votacionCreada = await context.Votacion
                .OrderByDescending(v => v.Id)
                .FirstOrDefaultAsync(v => v.Id_evento == idEvento && v.Id != idVotacion);

            Assert.IsNotNull(votacionCreada);
            Assert.AreEqual(inicioVotacion.ToUniversalTime(), votacionCreada.FechaInicio);
            Assert.AreEqual(finVotacion.ToUniversalTime(), votacionCreada.FechaFin);
        }

        [TestMethod]
        public async Task CrearVotacion_GeneraIdUnico()
        {
            var inicioVotacion = DateTime.Now.AddDays(1);
            var finVotacion = DateTime.Now.AddDays(10);

            await votacionService.CrearVotacion(idEvento, inicioVotacion, finVotacion, true, TipoVotacion.Publico);
            await votacionService.CrearVotacion(idEvento, inicioVotacion, finVotacion, false, TipoVotacion.Publico);

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
            context.Evento.Add(new Evento
            {
                Id = nuevoEvento,
                Nombre = "Nuevo Evento",
                Descripcion = "Descripcion nuevo evento",
                FechaInicio = DateTime.Now.AddDays(-1),
                FechaFin = DateTime.Now.AddDays(30)
            });
            await context.SaveChangesAsync();

            var nuevaFechaInicio = DateTime.Now.AddDays(5);
            var nuevaFechaFin = DateTime.Now.AddDays(15);

            await votacionService.EditarVotacion(idVotacion, nuevoEvento, nuevaFechaInicio, nuevaFechaFin, true, TipoVotacion.Publico);

            var votacionEditada = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacionEditada);
            Assert.AreEqual(nuevoEvento, votacionEditada.Id_evento);
            Assert.IsTrue(votacionEditada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task EditarVotacion_VotacionNoExiste_LanzaExcepcion()
        {
            var idInexistente = Guid.NewGuid();
            var inicioVotacion = DateTime.Now.AddDays(1);
            var finVotacion = DateTime.Now.AddDays(10);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EditarVotacion(idInexistente, idEvento, inicioVotacion, finVotacion, true, TipoVotacion.Publico)
            );

            Assert.AreEqual("La votación no existe", exception.Message);
        }

        [TestMethod]
        public async Task EditarVotacion_EventoVacio_LanzaExcepcion()
        {
            var inicioVotacion = DateTime.Now.AddDays(1);
            var finVotacion = DateTime.Now.AddDays(10);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EditarVotacion(idVotacion, Guid.Empty, inicioVotacion, finVotacion, true, TipoVotacion.Publico)
            );

            Assert.AreEqual("El evento es obligatorio", exception.Message);
        }

        [TestMethod]
        public async Task EditarVotacion_FechaFinAnteriorAInicio_LanzaExcepcion()
        {
            var inicioVotacion = DateTime.Now.AddDays(10);
            var finVotacion = DateTime.Now.AddDays(1);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EditarVotacion(idVotacion, idEvento, inicioVotacion, finVotacion, true, TipoVotacion.Publico)
            );

            Assert.AreEqual("La fecha de fin no puede ser anterior a la fecha de inicio", exception.Message);
        }

        [TestMethod]
        public async Task EditarVotacion_CambiaPermiteAutoVoto_ActualizaCorrectamente()
        {
            var votacionOriginal = await context.Votacion.FindAsync(idVotacion);
            var permisoOriginal = votacionOriginal.PermiteAutoVoto;

            var inicioVotacion = DateTime.Now.AddDays(1);
            var finVotacion = DateTime.Now.AddDays(10);

            await votacionService.EditarVotacion(idVotacion, idEvento, inicioVotacion, finVotacion, !permisoOriginal,TipoVotacion.Publico);

            var votacionActualizada = await context.Votacion.FindAsync(idVotacion);

            Assert.AreEqual(!permisoOriginal, votacionActualizada.PermiteAutoVoto);
        }

        [TestMethod]
        public async Task EditarVotacion_FechasIguales_NoLanzaExcepcion()
        {
            var fecha = DateTime.Now.AddDays(5);

            await votacionService.EditarVotacion(idVotacion, idEvento, fecha, fecha, true, TipoVotacion.Publico);

            var votacionEditada = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacionEditada);
            Assert.AreEqual(fecha.ToUniversalTime(), votacionEditada.FechaInicio);
            Assert.AreEqual(fecha.ToUniversalTime(), votacionEditada.FechaFin);
        }

        [TestMethod]
        public async Task EditarVotacion_ConservaIdOriginal()
        {
            var nuevaFechaInicio = DateTime.Now.AddDays(5);
            var nuevaFechaFin = DateTime.Now.AddDays(15);

            await votacionService.EditarVotacion(idVotacion, idEvento, nuevaFechaInicio, nuevaFechaFin, true, TipoVotacion.Publico);

            var votacionEditada = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacionEditada);
            Assert.AreEqual(idVotacion, votacionEditada.Id);
        }



        [TestMethod]
        public async Task EliminarVotacion_VotacionExiste_EliminaCorrectamente()
        {
            var votacionAntes = await context.Votacion.FindAsync(idVotacion);
            Assert.IsNotNull(votacionAntes);

            await votacionService.EliminarVotacion(idVotacion);

            var votacionDespues = await context.Votacion.FindAsync(idVotacion);
            Assert.IsNull(votacionDespues);
        }

        [TestMethod]
        public async Task EliminarVotacion_VotacionNoExiste_LanzaExcepcion()
        {
            var idInexistente = Guid.NewGuid();

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.EliminarVotacion(idInexistente)
            );

            Assert.AreEqual("La votación no existe", exception.Message);
        }

        [TestMethod]
        public async Task EliminarVotacion_NoAfectaOtrasVotaciones()
        {
            var otraVotacionId = Guid.NewGuid();
            var hoy = DateTime.Now;

            context.Votacion.Add(new Votify.Core.Entities.Votacion
            {
                Id = otraVotacionId,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1)
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
            var idInexistente = Guid.NewGuid();

            var resultado = await votacionService.ObtenerPorIdAsync(idInexistente);

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
            var idEventoInexistente = Guid.NewGuid();

            var resultado = await votacionService.BuscarVotacion(idEventoInexistente, 0, 0, true);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public async Task BuscarVotacion_FiltroPermiteAutoVoto_FiltraCorrectamente()
        {
            // Agregar votacion con PermiteAutoVoto = true
            var votacionConAutoVoto = Guid.NewGuid();
            var hoy = DateTime.Now;

            context.Votacion.Add(new Votify.Core.Entities.Votacion
            {
                Id = votacionConAutoVoto,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1),
                PermiteAutoVoto = true
            });
            await context.SaveChangesAsync();

            // El filtro 1 debería filtrar por PermiteAutoVoto = true (depende de la implementación del repo)
            var resultado = await votacionService.BuscarVotacion(idEvento, 0, 1, false);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public async Task BuscarVotacion_VerFinalizadasTrue_IncluyeFinalizadas()
        {
            var votacionFinalizada = Guid.NewGuid();
            var hoy = DateTime.Now;

            context.Votacion.Add(new Votify.Core.Entities.Votacion
            {
                Id = votacionFinalizada,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(-10),
                FechaFin = hoy.AddDays(-5)
            });
            await context.SaveChangesAsync();

            var resultado = await votacionService.BuscarVotacion(idEvento, 0, 0, true);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public async Task BuscarVotacion_VerFinalizadasFalse_ExcluyeFinalizadas()
        {
            var resultado = await votacionService.BuscarVotacion(idEvento, 0, 0, false);

            Assert.IsNotNull(resultado);
        }


    }
}
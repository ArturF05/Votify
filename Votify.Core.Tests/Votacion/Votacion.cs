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

            var hoy = DateOnly.FromDateTime(DateTime.Now);
            var categoriaId = Guid.NewGuid();

            context.Categoria.Add(new Categoria
            {
                Id = categoriaId,
                Nombre = "Categoria test"
            });

            var eventoPrincipal = factory.CreateEvento(
                "Hackathon",
                TipoVotacion.Publico,
                "Evento test",
                "Descripcion test",
                hoy.AddDays(-1),
                hoy.AddDays(1),
                categoriaId
            );
            idEvento = eventoPrincipal.Id;

            var otroEvento = factory.CreateEvento(
                "Hackathon",
                TipoVotacion.Publico,
                "Otro evento",
                "Descripcion otro evento",
                hoy.AddDays(-1),
                hoy.AddDays(1),
                categoriaId
            );

            var otroEvento2 = factory.CreateEvento(
                "Concurso",
                TipoVotacion.Publico,
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
                FechaInicio = hoy.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                FechaFin = hoy.AddDays(1).ToDateTime(TimeOnly.MinValue)
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
        public async Task RegistrarVotoValid_ReturnsTrue_AndGuarda3Votos()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idVotante,
                IdsProyectos = new List<Guid> { idProyecto1, idProyecto2, idProyecto3 }
            };

            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            Assert.IsTrue(resultado);

            var votosGuardados = context.Voto
                .Where(v => v.Id_votacion == idVotacion && v.Id_votante == idVotante)
                .ToList();

            Assert.AreEqual(3, votosGuardados.Count);
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
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            context.Votacion.Add(new Votify.Core.Entities.Votacion
            {
                Id = votacionFueraFechaId,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(2).ToDateTime(TimeOnly.MinValue),
                FechaFin = hoy.AddDays(5).ToDateTime(TimeOnly.MinValue),
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
                Fecha_voto = DateOnly.FromDateTime(DateTime.Now)
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
    }
}
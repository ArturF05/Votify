using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Core.DTOs;
using Votify.Core.Entities;
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

            var votacionRepository = new VotacionRepository(context);
            var votoRepository = new VotoRepository(context);
            var proyectoRepository = new ProyectoRepository(context);

            votacionService = new VotacionService(
                votacionRepository,
                votoRepository,
                proyectoRepository
            );

            idEvento = Guid.NewGuid();
            idVotacion = Guid.NewGuid();
            idVotante = Guid.NewGuid();

            idProyecto1 = Guid.NewGuid();
            idProyecto2 = Guid.NewGuid();
            idProyecto3 = Guid.NewGuid();
            idProyectoOtroEvento = Guid.NewGuid();

            var hoy = DateOnly.FromDateTime(DateTime.Now);

            context.Evento.Add(new Evento
            {
                Id = idEvento,
                Nombre = "Evento test",
                Descripcion = "Descripcion test",
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1)
            });

            context.Evento.Add(new Evento
            {
                Id = Guid.NewGuid(),
                Nombre = "Otro evento",
                Descripcion = "Descripcion otro evento",
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1)
            });

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

            var idOtroEvento = Guid.NewGuid();
            context.Evento.Add(new Evento
            {
                Id = idOtroEvento,
                Nombre = "Otro evento 2",
                Descripcion = "Descripcion otro evento 2",
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(1)
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
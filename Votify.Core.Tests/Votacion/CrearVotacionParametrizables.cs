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
    public class VotacionParametrizablesTests
    {
        private AppDbContext context;
        private VotacionService votacionService;
        private ConcreteEventFactory factory;

        private Guid idEvento;
        private Guid idVotacion;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            context = new AppDbContext(options);
            factory = new ConcreteEventFactory();
            VotacionRepository votacionRepository = new VotacionRepository(context);
            VotoRepository votoRepository = new VotoRepository(context);
            ProyectoRepository proyectoRepository = new ProyectoRepository(context);
            DimensionRepository dimensionRepository = new DimensionRepository(context);

            votacionService = new VotacionService(
                votacionRepository,
                votoRepository,
                proyectoRepository,
                dimensionRepository
            );

            DateTime hoy = DateTime.UtcNow;
            Guid categoriaId = Guid.NewGuid();

            context.Categoria.Add(new Categoria
            {
                Id = categoriaId,
                Nombre = "Categoria test"
            });

            var evento = factory.CreateEvento(
                "Hackathon",
                "Evento test",
                "Descripcion test",
                hoy.AddDays(-1),
                hoy.AddDays(10),
                categoriaId
            );

            idEvento = evento.Id;

            context.Evento.Add(evento);

            idVotacion = Guid.NewGuid();

            context.Votacion.Add(new VotacionPublico
            {
                Id = idVotacion,
                Id_evento = idEvento,
                FechaInicio = hoy.AddDays(-1),
                FechaFin = hoy.AddDays(10),
                PermiteAutoVoto = false
            });

            context.SaveChanges();
        }
        [TestMethod]
        public async Task CrearVotacion_NumProyectosNoValido_LanzaExcepcion()
        {
            var inicio = DateTime.Now.AddDays(1);
            var fin = DateTime.Now.AddDays(5);

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.CrearVotacion(
                    idEvento,
                    inicio,
                    fin,
                    true,
                    TipoVotacion.Publico,
                    0
                )
            );

            Assert.AreEqual("Se tiene que poder votar mínimo 1 proyecto", exception.Message);
        }

        [TestMethod]
        public async Task Votacion_NoPermiteSuperarNumeroMaximoProyectos()
        {
            var votacionId = Guid.NewGuid();

            context.Votacion.Add(new VotacionPublico
            {
                Id = votacionId,
                Id_evento = idEvento,
                FechaInicio = DateTime.UtcNow.AddHours(-1),
                FechaFin = DateTime.UtcNow.AddHours(2),
                PermiteAutoVoto = true,
                NumProy = 2
            });
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();

            context.Proyecto.AddRange(
                new Proyecto
                {
                    Id = p1,
                    Id_evento = idEvento,
                    Titulo = "P1",
                    Descripcion = "Desc P1"
                },
                new Proyecto
                {
                    Id = p2,
                    Id_evento = idEvento,
                    Titulo = "P2",
                    Descripcion = "Desc P2"
                },
                new Proyecto
                {
                    Id = p3,
                    Id_evento = idEvento,
                    Titulo = "P3",
                    Descripcion = "Desc P3"
                }
            );

            await context.SaveChangesAsync();

            var dto = new EmitirVotoDTO
            {
                IdVotante = Guid.NewGuid(),
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = p1 },
                    new VotoDetalleDTO { IdProyecto = p2 },
                    new VotoDetalleDTO { IdProyecto = p3 } 
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(votacionId, dto)
            );

            Assert.AreEqual("No puedes votar más proyectos que el máximo permitido.", exception.Message);
        }
    }
}
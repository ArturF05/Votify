using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Factories;
using Votify.Core.Observer;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Votacion
{
    [TestClass]
    public class ResultadosEnVivoTests
    {
        private AppDbContext context;
        private VotacionService votacionService;
        private ConcreteEventFactory factory;

        private Guid idEvento;
        private Guid idVotacion;

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
            var usuarioRolRepository = new EventoUsuarioRolRepository(context);

            votacionService = new VotacionService(
                votacionRepository,
                votoRepository,
                proyectoRepository,
                dimensionRepository,
                usuarioRepository,
                usuarioRolRepository,
                new Mock<IEmailSender>().Object
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
                PermiteAutoVoto = false,
                NumProy = 3,
                EstadoVotacion = EstadoVotacion.Abierta
            });

            context.SaveChanges();
        }

        [TestMethod]
        public async Task ConfigurarResultadosTiempoReal_Activar_GuardaConfiguracion()
        {
            var votacion = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacion);

            votacion.VerRes = true;

            context.Votacion.Update(votacion);
            await context.SaveChangesAsync();

            var votacionGuardada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id == idVotacion);

            Assert.IsNotNull(votacionGuardada);
            Assert.IsTrue(votacionGuardada.VerRes);
        }

        [TestMethod]
        public async Task ConfigurarResultadosTiempoReal_Desactivar_GuardaConfiguracion()
        {
            var votacion = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacion);

            votacion.VerRes = true;
            await context.SaveChangesAsync();

            votacion.VerRes = false;

            context.Votacion.Update(votacion);
            await context.SaveChangesAsync();

            var votacionGuardada = await context.Votacion
                .FirstOrDefaultAsync(v => v.Id == idVotacion);

            Assert.IsNotNull(votacionGuardada);
            Assert.IsFalse(votacionGuardada.VerRes);
        }

        [TestMethod]
        public async Task ConfigurarResultadosTiempoReal_Activado_PersisteTrasRecargar()
        {
            var votacion = await context.Votacion.FindAsync(idVotacion);

            Assert.IsNotNull(votacion);

            votacion.VerRes = true;
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var votacionRecargada = await context.Votacion
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == idVotacion);

            Assert.IsNotNull(votacionRecargada);
            Assert.IsTrue(votacionRecargada.VerRes);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Votacion
{
    [TestClass]
    public class CriteriosVotacionTests
    {
        private VotacionService votacionService;
        private AppDbContext context;
        private Guid idVotacion;
        private Guid idEvento;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);

            var votacionRepo = new VotacionRepository(context);
            var votoRepo = new VotoRepository(context);
            var proyectoRepo = new ProyectoRepository(context);
            var dimensionRepo = new DimensionRepository(context);
            var usuarioRepository = new UsuarioRepository(context);
            var eventoUsuarioRolRepository = new EventoUsuarioRolRepository(context);

            votacionService = new VotacionService(votacionRepo, votoRepo, proyectoRepo, dimensionRepo, usuarioRepository, eventoUsuarioRolRepository);

            idEvento = Guid.NewGuid();
            idVotacion = Guid.NewGuid();

            context.Votacion.Add(new VotacionPublico
            {
                Id = idVotacion,
                Id_evento = idEvento,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(1),
                PermiteAutoVoto = true
            });
            context.SaveChanges();
        }

        [TestMethod]
        public async Task EditarVotacion_ActualizaCriteriosEnBaseDeDatos()
        {
            // Test para verificar que el servicio guarda los cambios de los criterios
            var idExistente = Guid.NewGuid();
            context.DimensionesDeValor.Add(new DimensionesDeValor
            {
                Id = idExistente,
                Id_votacion = idVotacion,
                Nombre = "Viejo",
                Descripcion = "Criterio viejo",
                Peso = 100
            });
            await context.SaveChangesAsync();

            var dtos = new List<CrearCriterioDTO>{
                 new CrearCriterioDTO
                    {
                        Id = idExistente,
                        Nombre = "Viejo",
                        Descripcion = "Actualizado",
                        Peso = 30
                    },
                    new CrearCriterioDTO
                    {
                        Id = Guid.Empty,
                        Nombre = "Nuevo",
                        Descripcion = "Nuevo criterio",
                        Peso = 70
                    }
             };

            await votacionService.EditarVotacion(idVotacion, idEvento, DateTime.Now, DateTime.Now.AddDays(1), true, TipoVotacion.Publico, dtos, 3);

            var resultados = await context.DimensionesDeValor.Where(d => d.Id_votacion == idVotacion).ToListAsync();
            Assert.AreEqual(2, resultados.Count);
            Assert.IsTrue(resultados.Any(r => r.Peso == 30));
        }


        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
    
}
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

            votacionService = new VotacionService(
                votacionRepo,
                votoRepo,
                proyectoRepo,
                dimensionRepo,
                usuarioRepository,
                eventoUsuarioRolRepository
            );

            idEvento = Guid.NewGuid();
            idVotacion = Guid.NewGuid();

            context.Evento.Add(new Evento
            {
                Id = idEvento,
                Nombre = "Evento test",
                Descripcion = "Evento de prueba",
                FechaInicio = DateTime.UtcNow.AddDays(-1),
                FechaFin = DateTime.UtcNow.AddDays(10)
            });

            context.Votacion.Add(new VotacionPublico
            {
                Id = idVotacion,
                Id_evento = idEvento,
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddDays(1),
                PermiteAutoVoto = true,
                NumProy = 3,
                EstadoVotacion = EstadoVotacion.Abierta
            });

            context.SaveChanges();
        }

        [TestMethod]
        public async Task EditarVotacion_ActualizaCriteriosEnBaseDeDatos()
        {
            var idExistente = Guid.NewGuid();
            var jurados = new List<Guid>();

            context.DimensionesDeValor.Add(new DimensionesDeValor
            {
                Id = idExistente,
                Id_votacion = idVotacion,
                Nombre = "Viejo",
                Descripcion = "Criterio viejo",
                Peso = 100
            });

            await context.SaveChangesAsync();

            var dtos = new List<CrearCriterioDTO>
            {
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

            await votacionService.EditarVotacion(idVotacion, idEvento, DateTime.Now, DateTime.Now.AddDays(1), true, TipoVotacion.Publico, dtos, 3, true, true, jurados,EstadoVotacion.Abierta);

            

            await votacionService.EditarVotacion(idVotacion, idEvento, DateTime.Now, DateTime.Now.AddDays(1), true, TipoVotacion.Publico, jurados,EstadoVotacion.Abierta, dtos, 3);

            var resultados = await context.DimensionesDeValor.Where(d => d.Id_votacion == idVotacion).ToListAsync();
            Assert.AreEqual(2, resultados.Count);
            Assert.IsTrue(resultados.Any(r => r.Peso == 30));
            Assert.IsTrue(resultados.Any(r => r.Nombre == "Nuevo"));
        }

        [TestMethod]
        public async Task Dashboard_CriteriosSinComentarios_AparecenIgualmente()
        {
            var idVotacion = Guid.NewGuid();

            var criterioConComentarios = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Nombre = "Innovación",
                Descripcion = "Originalidad del proyecto",
                Peso = 50
            };

            var criterioSinComentarios = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Nombre = "Presentación",
                Descripcion = "Claridad al exponer",
                Peso = 50
            };

            var voto = new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Comentario = "Comentario general"
            };

            var votoCriterio = new VotoCriterio
            {
                Id = Guid.NewGuid(),
                Id_voto = voto.Id,
                Id_criterio = criterioConComentarios.Id,
                Comentario = "Muy buena innovación"
            };

            context.DimensionesDeValor.AddRange(criterioConComentarios, criterioSinComentarios);
            context.Voto.Add(voto);
            context.VotoCriterio.Add(votoCriterio);

            await context.SaveChangesAsync();

            var resultado = await context.DimensionesDeValor
                .Where(c => c.Id_votacion == idVotacion)
                .Select(c => new
                {
                    Criterio = c,
                    Comentarios = context.VotoCriterio
                        .Where(vc => vc.Id_criterio == c.Id)
                        .Select(vc => vc.Comentario)
                        .ToList()
                })
                .ToListAsync();

            Assert.AreEqual(2, resultado.Count);

            var sinComentarios = resultado.First(r => r.Criterio.Id == criterioSinComentarios.Id);

            Assert.IsNotNull(sinComentarios);
            Assert.AreEqual(0, sinComentarios.Comentarios.Count);
        }

        [TestMethod]
        public async Task Dashboard_ComentariosAgrupadosPorCriterio_NoSeMezclan()
        {
            var idVotacion = Guid.NewGuid();

            var criterioInnovacion = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Nombre = "Innovación",
                Descripcion = "Originalidad del proyecto",
                Peso = 50
            };

            var criterioPresentacion = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Nombre = "Presentación",
                Descripcion = "Claridad al exponer",
                Peso = 50
            };

            var voto = new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Comentario = "Comentario general"
            };

            var votoCriterioInnovacion1 = new VotoCriterio
            {
                Id = Guid.NewGuid(),
                Id_voto = voto.Id,
                Id_criterio = criterioInnovacion.Id,
                Comentario = "Idea muy original"
            };

            var votoCriterioInnovacion2 = new VotoCriterio
            {
                Id = Guid.NewGuid(),
                Id_voto = voto.Id,
                Id_criterio = criterioInnovacion.Id,
                Comentario = "La propuesta es innovadora"
            };

            var votoCriterioPresentacion = new VotoCriterio
            {
                Id = Guid.NewGuid(),
                Id_voto = voto.Id,
                Id_criterio = criterioPresentacion.Id,
                Comentario = "La exposición fue clara"
            };

            context.DimensionesDeValor.AddRange(criterioInnovacion, criterioPresentacion);
            context.Voto.Add(voto);
            context.VotoCriterio.AddRange(
                votoCriterioInnovacion1,
                votoCriterioInnovacion2,
                votoCriterioPresentacion
            );

            await context.SaveChangesAsync();

            var resultado = await context.DimensionesDeValor
                .Where(c => c.Id_votacion == idVotacion)
                .Select(c => new
                {
                    Criterio = c.Nombre,
                    Comentarios = context.VotoCriterio
                        .Where(vc => vc.Id_criterio == c.Id)
                        .Select(vc => vc.Comentario)
                        .ToList()
                })
                .ToListAsync();

            var innovacion = resultado.First(r => r.Criterio == "Innovación");
            var presentacion = resultado.First(r => r.Criterio == "Presentación");

            Assert.AreEqual(2, innovacion.Comentarios.Count);
            Assert.IsTrue(innovacion.Comentarios.Contains("Idea muy original"));
            Assert.IsTrue(innovacion.Comentarios.Contains("La propuesta es innovadora"));
            Assert.IsFalse(innovacion.Comentarios.Contains("La exposición fue clara"));

            Assert.AreEqual(1, presentacion.Comentarios.Count);
            Assert.IsTrue(presentacion.Comentarios.Contains("La exposición fue clara"));
            Assert.IsFalse(presentacion.Comentarios.Contains("Idea muy original"));
        }

        [TestMethod]
        public async Task DashboardUsuario_CalculaPuntuacionGlobal_MediasPorCriterio_YNumeroDeVotos()
        {
            var idUsuarioEvaluado = Guid.NewGuid();
            var idVotacion = Guid.NewGuid();

            var criterioInnovacion = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Nombre = "Innovación",
                Descripcion = "Originalidad del proyecto",
                Peso = 50
            };

            var criterioPresentacion = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Nombre = "Presentación",
                Descripcion = "Claridad al exponer",
                Peso = 50
            };

            var voto1 = new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_votante = idUsuarioEvaluado
            };

            var voto2 = new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_votante = idUsuarioEvaluado
            };

            context.DimensionesDeValor.AddRange(criterioInnovacion, criterioPresentacion);
            context.Voto.AddRange(voto1, voto2);

            context.VotoCriterio.AddRange(
                new VotoCriterio
                {
                    Id = Guid.NewGuid(),
                    Id_voto = voto1.Id,
                    Id_criterio = criterioInnovacion.Id,
                    Nota = 8
                },
                new VotoCriterio
                {
                    Id = Guid.NewGuid(),
                    Id_voto = voto1.Id,
                    Id_criterio = criterioPresentacion.Id,
                    Nota = 6
                },
                new VotoCriterio
                {
                    Id = Guid.NewGuid(),
                    Id_voto = voto2.Id,
                    Id_criterio = criterioInnovacion.Id,
                    Nota = 10
                },
                new VotoCriterio
                {
                    Id = Guid.NewGuid(),
                    Id_voto = voto2.Id,
                    Id_criterio = criterioPresentacion.Id,
                    Nota = 8
                }
            );

            await context.SaveChangesAsync();

            var votosUsuario = await context.Voto
                .Include(v => v.VotosCriterio)
                .Where(v => v.Id_votante == idUsuarioEvaluado)
                .ToListAsync();

            var puntuaciones = votosUsuario
                .SelectMany(v => v.VotosCriterio)
                .Where(vc => vc.Nota != null)
                .Select(vc => vc.Nota)
                .ToList();

            var puntuacionGlobal = puntuaciones.Average();

            var mediasPorCriterio = await context.VotoCriterio
                .Where(vc => vc.Nota != null)
                .Where(vc => votosUsuario.Select(v => v.Id).Contains(vc.Id_voto))
                .GroupBy(vc => vc.Id_criterio)
                .Select(g => new
                {
                    CriterioId = g.Key,
                    Media = g.Average(vc => vc.Nota),
                    NumeroVotos = g.Count()
                })
                .ToListAsync();

            var numeroVotosRecibidos = votosUsuario.Count;

            Assert.AreEqual(4, puntuaciones.Count);
            Assert.AreEqual(8, puntuacionGlobal);

            Assert.AreEqual(2, numeroVotosRecibidos);

            var mediaInnovacion = mediasPorCriterio.First(m => m.CriterioId == criterioInnovacion.Id);
            var mediaPresentacion = mediasPorCriterio.First(m => m.CriterioId == criterioPresentacion.Id);

            Assert.AreEqual(9, mediaInnovacion.Media);
            Assert.AreEqual(7, mediaPresentacion.Media);

            Assert.AreEqual(2, mediaInnovacion.NumeroVotos);
            Assert.AreEqual(2, mediaPresentacion.NumeroVotos);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
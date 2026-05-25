using Moq;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Services;

namespace Votify.Core.Tests.DashboardCompetidor
{
    [TestClass]
    public class DashboardServiceTests
    {
        private Mock<IEventoRepository> _eventoRepo = null!;
        private Mock<IVotacionRepository> _votacionRepo = null!;
        private Mock<IProyectoRepository> _proyectoRepo = null!;
        private Mock<IDimensionRepository> _dimensionRepo = null!;
        private DashboardService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _eventoRepo = new Mock<IEventoRepository>();
            _votacionRepo = new Mock<IVotacionRepository>();
            _proyectoRepo = new Mock<IProyectoRepository>();
            _dimensionRepo = new Mock<IDimensionRepository>();
            _service = new DashboardService(
                _eventoRepo.Object,
                _votacionRepo.Object,
                _proyectoRepo.Object,
                _dimensionRepo.Object);
        }

        // ─── ObtenerDashboardEvento ───────────────────────────────────────────

        [TestMethod]
        public async Task ObtenerDashboardEvento_EventoNoExiste_RetornaNull()
        {
            var eventoId = Guid.NewGuid();
            _eventoRepo.Setup(r => r.ObtenerEventoPorIdAsync(eventoId))
                       .ReturnsAsync((Evento?)null);

            var resultado = await _service.ObtenerDashboardEvento(eventoId);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task ObtenerDashboardEvento_EventoExiste_RetornaNombreEventoCorrecto()
        {
            var eventoId = Guid.NewGuid();
            ConfigurarEventoSinVotacion(eventoId, "Hackathon 2026");

            var resultado = await _service.ObtenerDashboardEvento(eventoId);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Hackathon 2026", resultado.NombreEvento);
        }

        [TestMethod]
        public async Task ObtenerDashboardEvento_SinVotacion_CriteriosVacios()
        {
            var eventoId = Guid.NewGuid();
            ConfigurarEventoSinVotacion(eventoId, "Test");

            var resultado = await _service.ObtenerDashboardEvento(eventoId);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(0, resultado.Criterios.Count);
        }

        [TestMethod]
        public async Task ObtenerDashboardEvento_SinProyectos_GanadorEsSinGanador()
        {
            var eventoId = Guid.NewGuid();
            ConfigurarEventoSinVotacion(eventoId, "Test");

            var resultado = await _service.ObtenerDashboardEvento(eventoId);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Sin ganador", resultado.TituloProyectoGanador);
        }

        // ─── ObtenerRankingProyectos ──────────────────────────────────────────

        [TestMethod]
        public async Task ObtenerRankingProyectos_SinProyectos_RetornaListaVacia()
        {
            var eventoId = Guid.NewGuid();
            _votacionRepo.Setup(r => r.ObtenerUltimaPorEventoAsync(eventoId))
                         .ReturnsAsync((global::Votify.Core.Entities.Votacion?)null);
            _proyectoRepo.Setup(r => r.ObtenerPorEventoIdAsync(eventoId))
                         .ReturnsAsync(new List<Proyecto>());

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public async Task ObtenerRankingProyectos_SinVotacion_TodosLosPoyectosConPuntuacionCero()
        {
            var eventoId = Guid.NewGuid();
            var proyectos = new List<Proyecto>
            {
                CrearProyecto("Proyecto A", eventoId),
                CrearProyecto("Proyecto B", eventoId)
            };

            _votacionRepo.Setup(r => r.ObtenerUltimaPorEventoAsync(eventoId))
                         .ReturnsAsync((global::Votify.Core.Entities.Votacion?)null);
            _proyectoRepo.Setup(r => r.ObtenerPorEventoIdAsync(eventoId))
                         .ReturnsAsync(proyectos);

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.All(p => p.PuntuacionTotal == 0m));
        }

        [TestMethod]
        public async Task ObtenerRankingProyectos_ConVotos_CalculaPuntuacionPonderadaCorrectamente()
        {
            var eventoId = Guid.NewGuid();
            var proyectoId = Guid.NewGuid();
            var criterioId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var criterio = new DimensionesDeValor
            {
                Id = criterioId, Nombre = "Innovación", Descripcion = "Test", Peso = 100
            };
            var votacion = new VotacionPublico
            {
                Id = votacionId, Id_evento = eventoId,
                DimensionesDeValores = new List<DimensionesDeValor> { criterio }
            };
            var proyecto = CrearProyecto("Proyecto A", eventoId, proyectoId);
            var voto = CrearVoto(proyectoId, votacionId, votacion, "Juez1",
                new[] { (criterioId, 8) });

            ConfigurarReposConVotacion(eventoId, votacion, new[] { proyecto }, new[] { voto });

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(8m, resultado[0].PuntuacionTotal); // 8 × (100/100) = 8
        }

        [TestMethod]
        public async Task ObtenerRankingProyectos_DosCriterios_PuntuacionEsPonderada()
        {
            var eventoId = Guid.NewGuid();
            var proyectoId = Guid.NewGuid();
            var criterio1Id = Guid.NewGuid();
            var criterio2Id = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var criterio1 = new DimensionesDeValor { Id = criterio1Id, Nombre = "Innov.", Descripcion = "T", Peso = 60 };
            var criterio2 = new DimensionesDeValor { Id = criterio2Id, Nombre = "Pres.", Descripcion = "T", Peso = 40 };
            var votacion = new VotacionPublico
            {
                Id = votacionId, Id_evento = eventoId,
                DimensionesDeValores = new List<DimensionesDeValor> { criterio1, criterio2 }
            };
            var proyecto = CrearProyecto("Proyecto A", eventoId, proyectoId);
            var voto = CrearVoto(proyectoId, votacionId, votacion, "Juez",
                new[] { (criterio1Id, 10), (criterio2Id, 5) });

            ConfigurarReposConVotacion(eventoId, votacion, new[] { proyecto }, new[] { voto });

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            // 10 × 0.6 + 5 × 0.4 = 6 + 2 = 8
            Assert.AreEqual(8m, resultado[0].PuntuacionTotal);
        }

        [TestMethod]
        public async Task ObtenerRankingProyectos_VariosProyectos_OrdenadoDescendente()
        {
            var eventoId = Guid.NewGuid();
            var proyecto1Id = Guid.NewGuid();
            var proyecto2Id = Guid.NewGuid();
            var criterioId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var criterio = new DimensionesDeValor { Id = criterioId, Nombre = "C", Descripcion = "T", Peso = 100 };
            var votacion = new VotacionPublico
            {
                Id = votacionId, Id_evento = eventoId,
                DimensionesDeValores = new List<DimensionesDeValor> { criterio }
            };
            var proyecto1 = CrearProyecto("Bajo", eventoId, proyecto1Id);
            var proyecto2 = CrearProyecto("Alto", eventoId, proyecto2Id);
            var votos = new[]
            {
                CrearVoto(proyecto1Id, votacionId, votacion, "J", new[] { (criterioId, 4) }),
                CrearVoto(proyecto2Id, votacionId, votacion, "J", new[] { (criterioId, 9) })
            };

            ConfigurarReposConVotacion(eventoId, votacion, new[] { proyecto1, proyecto2 }, votos);

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado[0].PuntuacionTotal >= resultado[1].PuntuacionTotal);
            Assert.AreEqual(proyecto2Id, resultado[0].ProyectoId);
        }

        // ─── ObtenerProyectoGanador ───────────────────────────────────────────

        [TestMethod]
        public async Task ObtenerProyectoGanador_SinProyectos_RetornaNull()
        {
            var eventoId = Guid.NewGuid();
            _votacionRepo.Setup(r => r.ObtenerUltimaPorEventoAsync(eventoId))
                         .ReturnsAsync((global::Votify.Core.Entities.Votacion?)null);
            _proyectoRepo.Setup(r => r.ObtenerPorEventoIdAsync(eventoId))
                         .ReturnsAsync(new List<Proyecto>());

            var resultado = await _service.ObtenerProyectoGanador(eventoId);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task ObtenerProyectoGanador_ConProyectos_RetornaElDeMaximaPuntuacion()
        {
            var eventoId = Guid.NewGuid();
            var ganadorId = Guid.NewGuid();
            var segundoId = Guid.NewGuid();
            var criterioId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var criterio = new DimensionesDeValor { Id = criterioId, Nombre = "C", Descripcion = "T", Peso = 100 };
            var votacion = new VotacionPublico
            {
                Id = votacionId, Id_evento = eventoId,
                DimensionesDeValores = new List<DimensionesDeValor> { criterio }
            };
            var votos = new[]
            {
                CrearVoto(ganadorId, votacionId, votacion, "J", new[] { (criterioId, 10) }),
                CrearVoto(segundoId, votacionId, votacion, "J", new[] { (criterioId, 6) })
            };

            ConfigurarReposConVotacion(eventoId, votacion,
                new[] { CrearProyecto("Ganador", eventoId, ganadorId), CrearProyecto("Segundo", eventoId, segundoId) },
                votos);

            var resultado = await _service.ObtenerProyectoGanador(eventoId);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(ganadorId, resultado.ProyectoId);
        }

        // ─── ObtenerCriteriosVotacion ─────────────────────────────────────────

        [TestMethod]
        public async Task ObtenerCriteriosVotacion_SinVotacion_RetornaListaVacia()
        {
            var eventoId = Guid.NewGuid();
            _votacionRepo.Setup(r => r.ObtenerUltimaPorEventoAsync(eventoId))
                         .ReturnsAsync((global::Votify.Core.Entities.Votacion?)null);

            var resultado = await _service.ObtenerCriteriosVotacion(eventoId);

            Assert.AreEqual(0, resultado.Count);
        }

        [TestMethod]
        public async Task ObtenerCriteriosVotacion_ConVotacion_RetornaCriteriosCorrectos()
        {
            var eventoId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();
            var votacion = new VotacionPublico { Id = votacionId, Id_evento = eventoId };
            var criterios = new List<DimensionesDeValor?>
            {
                new DimensionesDeValor { Id = Guid.NewGuid(), Nombre = "Innovación", Descripcion = "Desc", Peso = 60 },
                new DimensionesDeValor { Id = Guid.NewGuid(), Nombre = "Presentación", Descripcion = "Desc", Peso = 40 }
            };

            _votacionRepo.Setup(r => r.ObtenerUltimaPorEventoAsync(eventoId)).ReturnsAsync(votacion);
            _dimensionRepo.Setup(r => r.ObtenerCriterioPorVotacionAsync(votacionId)).ReturnsAsync(criterios);

            var resultado = await _service.ObtenerCriteriosVotacion(eventoId);

            Assert.AreEqual(2, resultado.Count);
            Assert.AreEqual("Innovación", resultado[0].Nombre);
            Assert.AreEqual(60, resultado[0].Peso);
        }

        // ─── ComentariosAnonimos (lógica de autoría) ──────────────────────────

        [TestMethod]
        public async Task ObtenerRankingProyectos_ComentariosAnonimos_MuestraAnonimo()
        {
            var eventoId = Guid.NewGuid();
            var proyectoId = Guid.NewGuid();
            var criterioId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var criterio = new DimensionesDeValor { Id = criterioId, Nombre = "C", Descripcion = "T", Peso = 100 };
            var votacion = new VotacionPublico
            {
                Id = votacionId, Id_evento = eventoId, ComentariosAnonimos = true,
                DimensionesDeValores = new List<DimensionesDeValor> { criterio }
            };
            var proyecto = CrearProyecto("P", eventoId, proyectoId);
            var voto = CrearVotoConComentario(proyectoId, votacionId, votacion, "Carlos", "Excelente", criterioId, 8);

            ConfigurarReposConVotacion(eventoId, votacion, new[] { proyecto }, new[] { voto });

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            var comentarios = resultado[0].Criterios.SelectMany(c => c.Comentarios).ToList();
            Assert.IsTrue(comentarios.Any(c => c.StartsWith("Anónimo:")));
        }

        [TestMethod]
        public async Task ObtenerRankingProyectos_ComentariosNoAnonimos_MuestraNombreUsuario()
        {
            var eventoId = Guid.NewGuid();
            var proyectoId = Guid.NewGuid();
            var criterioId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var criterio = new DimensionesDeValor { Id = criterioId, Nombre = "C", Descripcion = "T", Peso = 100 };
            var votacion = new VotacionPublico
            {
                Id = votacionId, Id_evento = eventoId, ComentariosAnonimos = false,
                DimensionesDeValores = new List<DimensionesDeValor> { criterio }
            };
            var proyecto = CrearProyecto("P", eventoId, proyectoId);
            var voto = CrearVotoConComentario(proyectoId, votacionId, votacion, "María", "Muy bueno", criterioId, 9);

            ConfigurarReposConVotacion(eventoId, votacion, new[] { proyecto }, new[] { voto });

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            var comentarios = resultado[0].Criterios.SelectMany(c => c.Comentarios).ToList();
            Assert.IsTrue(comentarios.Any(c => c.StartsWith("María:")));
        }

        [TestMethod]
        public async Task ObtenerRankingProyectos_UsuarioSinNombre_MuestraJuez()
        {
            var eventoId = Guid.NewGuid();
            var proyectoId = Guid.NewGuid();
            var criterioId = Guid.NewGuid();
            var votacionId = Guid.NewGuid();

            var criterio = new DimensionesDeValor { Id = criterioId, Nombre = "C", Descripcion = "T", Peso = 100 };
            var votacion = new VotacionPublico
            {
                Id = votacionId, Id_evento = eventoId, ComentariosAnonimos = false,
                DimensionesDeValores = new List<DimensionesDeValor> { criterio }
            };
            var proyecto = CrearProyecto("P", eventoId, proyectoId);
            var voto = CrearVotoConComentario(proyectoId, votacionId, votacion, "", "Bien hecho", criterioId, 7);

            ConfigurarReposConVotacion(eventoId, votacion, new[] { proyecto }, new[] { voto });

            var resultado = await _service.ObtenerRankingProyectos(eventoId);

            var comentarios = resultado[0].Criterios.SelectMany(c => c.Comentarios).ToList();
            Assert.IsTrue(comentarios.Any(c => c.StartsWith("Juez:")));
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        private void ConfigurarEventoSinVotacion(Guid eventoId, string nombre)
        {
            var evento = new EventoHackathon { Id = eventoId, Nombre = nombre };
            _eventoRepo.Setup(r => r.ObtenerEventoPorIdAsync(eventoId)).ReturnsAsync(evento);
            _votacionRepo.Setup(r => r.ObtenerUltimaPorEventoAsync(eventoId)).ReturnsAsync((global::Votify.Core.Entities.Votacion?)null);
            _proyectoRepo.Setup(r => r.ObtenerPorEventoIdAsync(eventoId)).ReturnsAsync(new List<Proyecto>());
            _eventoRepo.Setup(r => r.ObtenerVotosDeEventoAsync(eventoId)).ReturnsAsync(new List<Voto>());
        }

        private void ConfigurarReposConVotacion(Guid eventoId, global::Votify.Core.Entities.Votacion votacion, Proyecto[] proyectos, Voto[] votos)
        {
            var evento = new EventoHackathon { Id = eventoId, Nombre = "Test" };
            _eventoRepo.Setup(r => r.ObtenerEventoPorIdAsync(eventoId)).ReturnsAsync(evento);
            _votacionRepo.Setup(r => r.ObtenerUltimaPorEventoAsync(eventoId)).ReturnsAsync(votacion);
            _proyectoRepo.Setup(r => r.ObtenerPorEventoIdAsync(eventoId)).ReturnsAsync(proyectos.ToList());
            _eventoRepo.Setup(r => r.ObtenerVotosDeEventoAsync(eventoId)).ReturnsAsync(votos.ToList());
        }

        private static Proyecto CrearProyecto(string titulo, Guid eventoId, Guid? id = null) =>
            new Proyecto
            {
                Id = id ?? Guid.NewGuid(),
                Titulo = titulo,
                Descripcion = "",
                Id_evento = eventoId,
                ProyectoParticipantes = new List<ProyectoParticipantes>()
            };

        private static Voto CrearVoto(Guid proyectoId, Guid votacionId, global::Votify.Core.Entities.Votacion votacion,
            string nombreUsuario, (Guid criterioId, int nota)[] puntuaciones)
        {
            var voto = new Voto
            {
                Id = Guid.NewGuid(),
                Id_proyecto = proyectoId,
                Id_votacion = votacionId,
                Votacion = votacion,
                Usuario = new Usuario { Nombre = nombreUsuario },
                VotosCriterio = new List<VotoCriterio>()
            };
            foreach (var (criterioId, nota) in puntuaciones)
            {
                var vc = new VotoCriterio
                {
                    Id = Guid.NewGuid(),
                    Id_criterio = criterioId,
                    Nota = nota,
                    Voto = voto
                };
                voto.VotosCriterio.Add(vc);
            }
            return voto;
        }

        private static Voto CrearVotoConComentario(Guid proyectoId, Guid votacionId, global::Votify.Core.Entities.Votacion votacion,
            string nombreUsuario, string comentario, Guid criterioId, int nota)
        {
            var voto = new Voto
            {
                Id = Guid.NewGuid(),
                Id_proyecto = proyectoId,
                Id_votacion = votacionId,
                Votacion = votacion,
                Usuario = new Usuario { Nombre = nombreUsuario },
                VotosCriterio = new List<VotoCriterio>()
            };
            var vc = new VotoCriterio
            {
                Id = Guid.NewGuid(),
                Id_criterio = criterioId,
                Nota = nota,
                Comentario = comentario,
                Voto = voto
            };
            voto.VotosCriterio.Add(vc);
            return voto;
        }
    }
}

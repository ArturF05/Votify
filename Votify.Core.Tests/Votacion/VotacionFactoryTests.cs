using Moq;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Factories;
using Votify.Core.Interfaces;
using Votify.Core.Observer;
using Votify.Core.Services;

namespace Votify.Core.Tests.VotacionTests
{
    [TestClass]
    public class VotacionFactoryTests
    {
        private readonly ConcreteVotacionFactory _factory = new();
        private readonly DateTime _inicio = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        private readonly DateTime _fin = new DateTime(2026, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        private readonly Guid _eventoId = Guid.NewGuid();

        // ─── Tipo de votación creado ──────────────────────────────────────────

        [TestMethod]
        public void CrearVotacion_TipoPublico_DevuelveVotacionPublico()
        {
            var resultado = _factory.CrearVotacion(
                _inicio, _fin, false, _eventoId, TipoVotacion.Publico,
                3, false, false, false, false, EstadoVotacion.Abierta);

            Assert.IsInstanceOfType(resultado, typeof(VotacionPublico));
        }

        [TestMethod]
        public void CrearVotacion_TipoJurado_DevuelveVotacionJurado()
        {
            var resultado = _factory.CrearVotacion(
                _inicio, _fin, false, _eventoId, TipoVotacion.Jurado,
                3, false, false, false, false, EstadoVotacion.Abierta);

            Assert.IsInstanceOfType(resultado, typeof(VotacionJurado));
        }

        [TestMethod]
        public void CrearVotacion_TipoMixto_DevuelveVotacionMixta()
        {
            var resultado = _factory.CrearVotacion(
                _inicio, _fin, false, _eventoId, TipoVotacion.Mixto,
                3, false, false, false, false, EstadoVotacion.Abierta);

            Assert.IsInstanceOfType(resultado, typeof(VotacionMixta));
        }

        [TestMethod]
        public void CrearVotacion_TipoNoReconocido_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _factory.CrearVotacion(
                    _inicio, _fin, false, _eventoId, (TipoVotacion)99,
                    3, false, false, false, false, EstadoVotacion.Abierta));
        }

        // ─── Propiedades asignadas ────────────────────────────────────────────

        [TestMethod]
        public void CrearVotacion_PropiedadesBasicasAsignadasCorrectamente()
        {
            var resultado = _factory.CrearVotacion(
                _inicio, _fin, true, _eventoId, TipoVotacion.Publico,
                5, true, true, true, true, EstadoVotacion.Abierta);

            Assert.AreEqual(_eventoId, resultado.Id_evento);
            Assert.AreEqual(_inicio.ToUniversalTime(), resultado.FechaInicio);
            Assert.AreEqual(_fin.ToUniversalTime(), resultado.FechaFin);
            Assert.IsTrue(resultado.PermiteAutoVoto);
            Assert.AreEqual(5, resultado.NumProy);
            Assert.IsTrue(resultado.PermiteComentarios);
            Assert.IsTrue(resultado.ComentariosObligatorios);
            Assert.IsTrue(resultado.ComentariosAnonimos);
            Assert.IsTrue(resultado.VerRes);
            Assert.AreEqual(EstadoVotacion.Abierta, resultado.EstadoVotacion);
        }

        [TestMethod]
        public void CrearVotacion_GeneraNuevoIdDistintoEnCadaLlamada()
        {
            var v1 = _factory.CrearVotacion(_inicio, _fin, false, _eventoId, TipoVotacion.Publico,
                1, false, false, false, false, EstadoVotacion.Abierta);
            var v2 = _factory.CrearVotacion(_inicio, _fin, false, _eventoId, TipoVotacion.Publico,
                1, false, false, false, false, EstadoVotacion.Abierta);

            Assert.AreNotEqual(v1.Id, v2.Id);
        }

        [TestMethod]
        public void RecrearVotacion_MantieneIdOriginal()
        {
            var idOriginal = Guid.NewGuid();

            var resultado = _factory.RecrearVotacion(
                idOriginal, _inicio, _fin, false, _eventoId, TipoVotacion.Jurado,
                2, false, false, false, false, EstadoVotacion.Cerrada);

            Assert.AreEqual(idOriginal, resultado.Id);
        }
    }

    // ─── VotacionService: validaciones ────────────────────────────────────────

    [TestClass]
    public class VotacionServiceValidacionTests
    {
        private Mock<IVotacionRepository> _votacionRepo = null!;
        private Mock<IVotoRepository> _votoRepo = null!;
        private Mock<IProyectoRepository> _proyectoRepo = null!;
        private Mock<IDimensionRepository> _dimensionRepo = null!;
        private Mock<IUsuarioRepository> _usuarioRepo = null!;
        private Mock<IEventoUsuarioRolRepository> _eurRepo = null!;
        private VotacionService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _votacionRepo = new Mock<IVotacionRepository>();
            _votoRepo = new Mock<IVotoRepository>();
            _proyectoRepo = new Mock<IProyectoRepository>();
            _dimensionRepo = new Mock<IDimensionRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _eurRepo = new Mock<IEventoUsuarioRolRepository>();
            _service = new VotacionService(
                _votacionRepo.Object, _votoRepo.Object, _proyectoRepo.Object,
                _dimensionRepo.Object, _usuarioRepo.Object, _eurRepo.Object,
                new Mock<IEmailSender>().Object);
        }

        [TestMethod]
        public async Task CrearVotacion_IdEventoVacio_LanzaExcepcion()
        {
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CrearVotacion(
                    Guid.Empty, null,
                    DateTime.UtcNow, DateTime.UtcNow.AddDays(1),
                    false, TipoVotacion.Publico, 1,
                    false, false, false, false,
                    new List<Guid>(), EstadoVotacion.Abierta, new List<CrearCriterioDTO>()));
        }

        [TestMethod]
        public async Task CrearVotacion_FechaFinMenorQueFechaInicio_LanzaExcepcion()
        {
            var eventoId = Guid.NewGuid();
            var inicio = DateTime.UtcNow.AddDays(5);
            var fin = DateTime.UtcNow.AddDays(1);

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CrearVotacion(
                    eventoId, null, inicio, fin,
                    false, TipoVotacion.Publico, 1,
                    false, false, false, false,
                    new List<Guid>(), EstadoVotacion.Abierta, new List<CrearCriterioDTO>()));
        }

        [TestMethod]
        public async Task CrearVotacion_NumProyCero_LanzaExcepcion()
        {
            var eventoId = Guid.NewGuid();

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CrearVotacion(
                    eventoId, null,
                    DateTime.UtcNow, DateTime.UtcNow.AddDays(1),
                    false, TipoVotacion.Publico, 0,
                    false, false, false, false,
                    new List<Guid>(), EstadoVotacion.Abierta, new List<CrearCriterioDTO>()));
        }

        [TestMethod]
        public async Task CrearVotacion_PermiteComentariosFalso_ComentariosAnonimosYObligatoriosSeIgnoran()
        {
            var eventoId = Guid.NewGuid();
            global::Votify.Core.Entities.Votacion? votacionGuardada = null;

            _dimensionRepo.Setup(r => r.CategoriaEventoPerteneceAlEventoAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                          .ReturnsAsync(true);
            _votacionRepo.Setup(r => r.CrearVotacionAsync(It.IsAny<global::Votify.Core.Entities.Votacion>()))
                         .Callback<global::Votify.Core.Entities.Votacion>(v => votacionGuardada = v)
                         .Returns(Task.CompletedTask);

            await _service.CrearVotacion(
                eventoId, null,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(1),
                false, TipoVotacion.Publico, 1,
                permiteComentarios: false,
                comentariosObligatorios: true,  // debe ignorarse
                comentariosAnonimos: true,       // debe ignorarse
                verRes: false,
                new List<Guid>(), EstadoVotacion.Abierta, new List<CrearCriterioDTO>());

            Assert.IsNotNull(votacionGuardada);
            Assert.IsFalse(votacionGuardada!.ComentariosAnonimos);
            Assert.IsFalse(votacionGuardada.ComentariosObligatorios);
        }

        [TestMethod]
        public async Task CrearVotacion_TipoPublico_GuardaVotacionPublico()
        {
            var eventoId = Guid.NewGuid();
            global::Votify.Core.Entities.Votacion? votacionGuardada = null;

            _dimensionRepo.Setup(r => r.CategoriaEventoPerteneceAlEventoAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                          .ReturnsAsync(true);
            _votacionRepo.Setup(r => r.CrearVotacionAsync(It.IsAny<global::Votify.Core.Entities.Votacion>()))
                         .Callback<global::Votify.Core.Entities.Votacion>(v => votacionGuardada = v)
                         .Returns(Task.CompletedTask);

            await _service.CrearVotacion(
                eventoId, null,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(1),
                false, TipoVotacion.Publico, 1,
                false, false, false, false,
                new List<Guid>(), EstadoVotacion.Abierta, new List<CrearCriterioDTO>());

            Assert.IsInstanceOfType(votacionGuardada, typeof(VotacionPublico));
        }

        [TestMethod]
        public async Task CrearVotacion_TipoJurado_GuardaVotacionJurado()
        {
            var eventoId = Guid.NewGuid();
            global::Votify.Core.Entities.Votacion? votacionGuardada = null;

            _dimensionRepo.Setup(r => r.CategoriaEventoPerteneceAlEventoAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                          .ReturnsAsync(true);
            _votacionRepo.Setup(r => r.CrearVotacionAsync(It.IsAny<global::Votify.Core.Entities.Votacion>()))
                         .Callback<global::Votify.Core.Entities.Votacion>(v => votacionGuardada = v)
                         .Returns(Task.CompletedTask);

            await _service.CrearVotacion(
                eventoId, null,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(1),
                false, TipoVotacion.Jurado, 1,
                false, false, false, false,
                new List<Guid>(), EstadoVotacion.Abierta, new List<CrearCriterioDTO>());

            Assert.IsInstanceOfType(votacionGuardada, typeof(VotacionJurado));
        }

        [TestMethod]
        public async Task CrearVotacion_TipoMixto_GuardaVotacionMixta()
        {
            var eventoId = Guid.NewGuid();
            global::Votify.Core.Entities.Votacion? votacionGuardada = null;

            _dimensionRepo.Setup(r => r.CategoriaEventoPerteneceAlEventoAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                          .ReturnsAsync(true);
            _votacionRepo.Setup(r => r.CrearVotacionAsync(It.IsAny<global::Votify.Core.Entities.Votacion>()))
                         .Callback<global::Votify.Core.Entities.Votacion>(v => votacionGuardada = v)
                         .Returns(Task.CompletedTask);

            await _service.CrearVotacion(
                eventoId, null,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(1),
                false, TipoVotacion.Mixto, 1,
                false, false, false, false,
                new List<Guid>(), EstadoVotacion.Abierta, new List<CrearCriterioDTO>());

            Assert.IsInstanceOfType(votacionGuardada, typeof(VotacionMixta));
        }
    }
}

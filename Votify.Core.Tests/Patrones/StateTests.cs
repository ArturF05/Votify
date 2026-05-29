using Moq;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Observer;
using Votify.Core.Services;
using Votify.Core.State;

namespace Votify.Core.Tests.Patrones
{
    [TestClass]
    public class StateTests
    {
        private static global::Votify.Core.Entities.Votacion CrearVotacion(EstadoVotacion estado)
        {
            return new VotacionPublico
            {
                Id = Guid.NewGuid(),
                Id_evento = Guid.NewGuid(),
                FechaInicio = DateTime.UtcNow.AddHours(-1),
                FechaFin = DateTime.UtcNow.AddHours(1),
                EstadoVotacion = estado,
                NumProy = 1
            };
        }

        [TestMethod]
        public void Factory_EstadoAbierta_DevuelveEstadoAbierta()
        {
            var factory = new EstadoVotacionFactory();

            var estado = factory.Crear(EstadoVotacion.Abierta);

            Assert.IsInstanceOfType(estado, typeof(EstadoAbierta));
        }

        [TestMethod]
        public void Factory_EstadoCerrada_DevuelveEstadoCerrada()
        {
            var factory = new EstadoVotacionFactory();

            var estado = factory.Crear(EstadoVotacion.Cerrada);

            Assert.IsInstanceOfType(estado, typeof(EstadoCerrada));
        }

        [TestMethod]
        public void Factory_EstadoPausada_DevuelveEstadoPausada()
        {
            var factory = new EstadoVotacionFactory();

            var estado = factory.Crear(EstadoVotacion.Pausada);

            Assert.IsInstanceOfType(estado, typeof(EstadoPausada));
        }

        [TestMethod]
        public void Factory_EstadoFinalizada_DevuelveEstadoFinalizada()
        {
            var factory = new EstadoVotacionFactory();

            var estado = factory.Crear(EstadoVotacion.Finalizada);

            Assert.IsInstanceOfType(estado, typeof(EstadoFinalizada));
        }

        [TestMethod]
        public void EstadoAbierta_DentroDelRangoDeFechas_PermiteVotar()
        {
            var estado = new EstadoAbierta();
            var votacion = CrearVotacion(EstadoVotacion.Abierta);

            estado.ValidarPuedeVotar(votacion, DateTime.UtcNow);
        }

        [TestMethod]
        public void EstadoAbierta_FueraDelRangoDeFechas_BloqueaVoto()
        {
            var estado = new EstadoAbierta();
            var votacion = CrearVotacion(EstadoVotacion.Abierta);

            Assert.ThrowsException<Exception>(() =>
                estado.ValidarPuedeVotar(votacion, DateTime.UtcNow.AddDays(2)));
        }

        [TestMethod]
        public void EstadoCerrada_BloqueaVoto()
        {
            var estado = new EstadoCerrada();
            var votacion = CrearVotacion(EstadoVotacion.Cerrada);

            Assert.ThrowsException<Exception>(() =>
                estado.ValidarPuedeVotar(votacion, DateTime.UtcNow));
        }

        [TestMethod]
        public void EstadoPausada_BloqueaVoto()
        {
            var estado = new EstadoPausada();
            var votacion = CrearVotacion(EstadoVotacion.Pausada);

            Assert.ThrowsException<Exception>(() =>
                estado.ValidarPuedeVotar(votacion, DateTime.UtcNow));
        }

        [TestMethod]
        public void EstadoFinalizada_BloqueaVoto()
        {
            var estado = new EstadoFinalizada();
            var votacion = CrearVotacion(EstadoVotacion.Finalizada);

            Assert.ThrowsException<Exception>(() =>
                estado.ValidarPuedeVotar(votacion, DateTime.UtcNow));
        }

        [TestMethod]
        public async Task RegistrarVoto_VotacionCerrada_BloqueaAntesDeGuardarVotos()
        {
            var votacion = CrearVotacion(EstadoVotacion.Cerrada);
            var votacionRepository = new Mock<IVotacionRepository>();
            var votoRepository = new Mock<IVotoRepository>();
            var proyectoRepository = new Mock<IProyectoRepository>();
            var dimensionRepository = new Mock<IDimensionRepository>();
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var eventoUsuarioRolRepository = new Mock<IEventoUsuarioRolRepository>();
            var emailSender = new Mock<IEmailSender>();

            votacionRepository
                .Setup(r => r.ObtenerPorIdAsync(votacion.Id))
                .ReturnsAsync(votacion);

            var service = new VotacionService(
                votacionRepository.Object,
                votoRepository.Object,
                proyectoRepository.Object,
                dimensionRepository.Object,
                usuarioRepository.Object,
                eventoUsuarioRolRepository.Object,
                emailSender.Object);

            var dto = new EmitirVotoDTO
            {
                IdEvento = votacion.Id_evento,
                IdVotante = Guid.NewGuid(),
                Votos = new List<VotoDetalleDTO>
                {
                    new()
                    {
                        IdProyecto = Guid.NewGuid(),
                        IdCategoriaEvento = Guid.NewGuid()
                    }
                }
            };

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                service.RegistrarVotoAsync(votacion.Id, dto));

            votoRepository.Verify(r => r.GuardarVotosAsync(It.IsAny<List<Voto>>()), Times.Never);
            proyectoRepository.Verify(r => r.ObtenerPorIdAsync(It.IsAny<List<Guid>>()), Times.Never);
        }
    }
}

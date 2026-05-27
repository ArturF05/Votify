using Votify.Core.Entities;

namespace Votify.Core.State
{
    public class EstadoVotacionFactory
    {
        public IEstadoVotacionState Crear(EstadoVotacion estado)
        {
            return estado switch
            {
                EstadoVotacion.Abierta => new EstadoAbierta(),
                EstadoVotacion.Cerrada => new EstadoCerrada(),
                EstadoVotacion.Pausada => new EstadoPausada(),
                EstadoVotacion.Finalizada => new EstadoFinalizada(),
                _ => throw new Exception("Estado de votación no reconocido")
            };
        }
    }
}

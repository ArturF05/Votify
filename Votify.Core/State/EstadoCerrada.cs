using Votify.Core.Entities;

namespace Votify.Core.State
{
    public class EstadoCerrada : IEstadoVotacionState
    {
        public void ValidarPuedeVotar(Votacion votacion, DateTime ahoraUtc)
        {
            throw new Exception("La votación está cerrada.");
        }
    }
}

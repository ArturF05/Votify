using Votify.Core.Entities;

namespace Votify.Core.State
{
    public class EstadoPausada : IEstadoVotacionState
    {
        public void ValidarPuedeVotar(Votacion votacion, DateTime ahoraUtc)
        {
            throw new Exception("La votación está pausada temporalmente.");
        }
    }
}

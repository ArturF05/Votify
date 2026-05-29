using Votify.Core.Entities;

namespace Votify.Core.State
{
    public class EstadoFinalizada : IEstadoVotacionState
    {
        public void ValidarPuedeVotar(Votacion votacion, DateTime ahoraUtc)
        {
            throw new Exception("La votacion ya ha finalizado.");
        }
    }
}

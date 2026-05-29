using Votify.Core.Entities;

namespace Votify.Core.State
{
    public class EstadoAbierta : IEstadoVotacionState
    {
        public void ValidarPuedeVotar(Votacion votacion, DateTime ahoraUtc)
        {
            if (ahoraUtc < votacion.FechaInicio || ahoraUtc > votacion.FechaFin)
                throw new Exception("La votacion no esta activa por fecha.");
        }
    }
}

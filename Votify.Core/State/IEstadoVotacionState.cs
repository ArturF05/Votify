using Votify.Core.Entities;

namespace Votify.Core.State
{
    public interface IEstadoVotacionState
    {
        void ValidarPuedeVotar(Votacion votacion, DateTime ahoraUtc);
    }
}

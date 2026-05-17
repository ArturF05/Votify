using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IVotacionRepository
    {
        Task<Votacion?> ObtenerPorIdAsync(Guid idVotacion);
        Task<List<Votacion>?> BuscarPorNombreAsync(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas);
        Task CrearVotacionAsync(Votacion votacion);
        Task EditarVotacionAsync(Votacion votacion);
        Task EliminarVotacionAsync(Votacion votacion);
        Task ReemplazarVotacionAsync(Votacion original, Votacion nuevaVotacion);
        Task<List<Voto>> ObtenerPorVotacionYUsuarioAsync(Guid votacionId, Guid usuarioId);
        Task<bool> YaVotoAsync(Guid idVotacion, Guid idVotante);
        Task<bool> YaVotoCategoriaAsync(Guid idVotacion, Guid idVotante, Guid idCategoriaEvento);
        Task<Votacion?> ObtenerUltimaPorEventoAsync(Guid eventoId);
        Task<List<Votacion>> ObtenerVotacionesEventos(List<Guid> idEvento);
        Task<List<Voto>> ObtenerVotosVotacion(Guid idVotacion);


    }
}

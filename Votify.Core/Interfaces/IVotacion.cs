using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IVotacion
    {
        Task<bool> RegistrarVotoAsync(Guid idVotacion, EmitirVotoDTO dto);
        Task<bool> YaVotoAsync(Guid votacionId, Guid usuarioId);
        Task<List<Voto>> ObtenerVotosUsuarioAsync(Guid votacionId, Guid usuarioId);
        Task<List<Votacion>> BuscarVotacion(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas);
        Task CrearVotacion(Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion,int NumProy, bool PermiteComentarios, bool ComentariosObligatorios, bool verRes,List<Guid> Jurados, EstadoVotacion estadoVotacion, List<CrearCriterioDTO> criterios);
        Task EditarVotacion(Guid Id, Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipovotacion, List<CrearCriterioDTO> criterios,int NumProy, bool permiteComentarios, bool comentariosObligatorios, bool verRes,List<Guid> Jurados, EstadoVotacion estadoVotacion);
        Task EliminarVotacion(Guid Id);
        Task<Votacion?> ObtenerPorIdAsync(Guid id);

    }
}

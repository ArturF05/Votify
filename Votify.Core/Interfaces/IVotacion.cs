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
        Task<List<Votacion>> BuscarVotacion(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas);
        Task CrearVotacion(Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion,int NumProy);
        Task EditarVotacion(Guid Id, Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipovotacion, List<CrearCriterioDTO> criterios);
        Task EliminarVotacion(Guid Id);
        Task<Votacion?> ObtenerPorIdAsync(Guid id);
    }
}

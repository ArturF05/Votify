using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IEventoRepository
    {
        Task<Evento> CrearEventoAsync(Evento evento);
        Task<List<Evento>?> BuscarPorNombreAsync(string nombre, Guid Id_Categoria, Guid idUsuario);
        Task EditarEventoAsync(Evento evento);
        Task EliminarEventoAsync(Evento evento);
        Task ArchivoEventoAsync(Evento evento, bool archivar);
        Task ApuntarseEventoAsync(Evento evento, Guid usuarioId, bool apuntar);
        Task<Evento?> ObtenerEventoPorIdAsync(Guid id);
        Task<List<Evento>> ObtenerTodos();
        Task<Evento?> ObtenerEventoDashboardAsync(Guid eventoId);
        Task<List<Voto>> ObtenerVotosDeEventoAsync(Guid eventoId);
        Task<Votacion?> ObtenerVotacionPorEventoAsync(Guid eventoId);
        Task AsignarOrganizadorAsync(Guid eventoId, Guid usuarioId);

    }
}

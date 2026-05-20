using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.DTOs;

namespace Votify.Core.Interfaces
{
    public interface IEvento
    {
        Task<Evento> CrearEvento(string titulo, Guid categoriaId, DateTime fecha_inicio, string descripcion, DateTime fecha_cierre,string tipo, Guid usuarioId, string? imagenUrl);
        Task EditarEvento(Guid eventoId, string titulo, Guid categoriaId, DateTime fechaInicio, string descripcion, DateTime fechaFin, string? imagenUrl);
        Task EliminarEvento(Guid eventoId);
        Task<List<Evento>?> BuscarEvento(string nombre, Guid Id_Categoria, Guid idUsuario);
        Task Archivo(Guid eventoId, bool archivar);
        Task ApuntarseAsync(Guid eventoId, Guid idUsuario, bool apuntar);
        Task<Evento?> ObtenerEventoPorIdAsync(Guid id);
        Task<List<Evento>> ObtenerTodos();
        Task<DashboardEventoDTO> ObtenerDashboardEventoAsync(Guid id);
        Task<int> NumeroVotosPorEvento(Guid idEvento);
        Task<List<Votify.Core.Entities.Evento>> ObtenerEventosOrganizador(Guid idUsuario);
        Task<EventoDetalleDTO?> ObtenerEventoDetalleAsync(Guid id);
    }
}

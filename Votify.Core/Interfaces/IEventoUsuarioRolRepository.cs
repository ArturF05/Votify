using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IEventoUsuarioRolRepository
    {
        Task<List<EventoUsuarioRol>> ObtenerPorEventoYUsuario(Guid eventoId, Guid usuarioId);
    }
}

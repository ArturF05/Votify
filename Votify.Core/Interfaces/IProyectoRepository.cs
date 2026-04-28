using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{

    public interface IProyectoRepository
    {
        Task<bool> CrearProyectoAsync(Proyecto proyecto);
        Task<List<Proyecto>> ObtenerPorIdAsync(List<Guid> idsProyectos);
        Task<List<Proyecto>> ObtenerPorUsuarioAsync(Guid usuarioId);
    }
}

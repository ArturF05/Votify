using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Interfaces
{
   
    public interface IProyectoRepository{
        Task<bool> CrearProyectoAsync(
        string nombre,
        string descripcion,
        DateTime? fechaInicio,
        DateTime? fechaFin
            );
        }
}

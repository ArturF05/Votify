using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Services;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IProyectoService
    {
        Task<bool> CrearProyecto(
            string nombre,
            string descripcion,
            DateTime? fechaInicio,
            DateTime? fechaFin
        );
    }
}

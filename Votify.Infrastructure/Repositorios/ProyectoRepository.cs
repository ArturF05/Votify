using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Interfaces;

namespace Votify.Infrastructure.Repositorios
{
    public class ProyectoRepository : IProyectoRepository
    {
        public async Task<bool> CrearProyectoAsync(string titulo, string descripcion)
        {
            // Aquí irá la lógica para guardar el proyecto en la base de datos
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class ProyectoService : IProyectoService
    {
        private readonly IProyectoRepository _proyectoRepository;


        public ProyectoService(IProyectoRepository proyectoRepository)
        {
            _proyectoRepository = proyectoRepository;
        }

        public async Task<bool> CrearProyecto(
             string nombre,
             string descripcion)
        {


            return await _proyectoRepository.CrearProyectoAsync(
                nombre,
                descripcion
            );

        }
    }
}

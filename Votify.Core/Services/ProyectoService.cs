using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    internal class ProyectoService : IProyectoService
    {
        private readonly IProyectoRepository _proyectoRepository;


        public ProyectoService(IProyectoRepository proyectoRepository)
        {
            _proyectoRepository = proyectoRepository;
        }

        public async Task<bool> CrearProyecto(
             string nombre,
             string descripcion,
             DateTime? fechaInicio,
             DateTime? fechaFin)
        {
            if (fechaFin < fechaInicio)
            {
                throw new Exception("La fecha fin no puede ser menor que la fecha inicio");
            }

            return await _proyectoRepository.CrearProyectoAsync(
                nombre,
                descripcion,
                fechaInicio,
                fechaFin
            );

        }
    }
}

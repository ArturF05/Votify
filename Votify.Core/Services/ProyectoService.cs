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

        public async Task <bool> CrearProyecto(string nombre, string descripcion)
        {
            nombre = nombre.Trim();
            descripcion = descripcion.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception(" Debe tener un nombre");
            }
            var proyecto = new Proyecto
            {
                Id = Guid.NewGuid(),
                Titulo = nombre,
                Descripcion = descripcion
            };
            return await _proyectoRepository.CrearProyectoAsync(proyecto);
        }
    }
}

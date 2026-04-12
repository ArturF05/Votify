using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class ProyectoService : IProyectoService
    {
        private readonly IProyectoRepository _proyectoRepository;
        private readonly IEventoRepository _eventoRepository;

        public ProyectoService(IProyectoRepository proyectoRepository,IEventoRepository eventoRepository){
            _proyectoRepository = proyectoRepository;
            _eventoRepository = eventoRepository;
        }

        public async Task<bool> CrearProyecto(string nombre, string descripcion, Guid idEvento){
            nombre = nombre?.Trim() ?? string.Empty;
            descripcion = descripcion?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new Exception("Debe tener un nombre");
            }

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new Exception("Debe tener una descripción");
            }

            var evento = await _eventoRepository.ObtenerEventoPorIdAsync(idEvento);

            if (evento == null)
            {
                throw new Exception("Debe seleccionar un evento válido");
            }

            var proyecto = new Proyecto
            {
                Id = Guid.NewGuid(),
                Titulo = nombre,
                Descripcion = descripcion,
                Id_evento = idEvento,
                Evento = evento
            };

            if (evento.Proyectos == null)
            {
                evento.Proyectos = new List<Proyecto>();
            }

            evento.Proyectos.Add(proyecto);

            return await _proyectoRepository.CrearProyectoAsync(proyecto);
        }
    }
}
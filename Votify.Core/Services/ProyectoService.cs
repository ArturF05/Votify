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

        public async Task<bool> CrearProyecto(string nombre, string descripcion, Guid idEvento, Guid idCreador){
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

            Proyecto proyecto =  new Proyecto
            {
                Id = Guid.NewGuid(),
                Titulo = nombre,
                Descripcion = descripcion,
                Id_evento = idEvento,
                Id_creador = idCreador,
                ProyectoParticipantes = new List<ProyectoParticipantes>()
            };
            proyecto.ProyectoParticipantes.Add(new ProyectoParticipantes
            {
                Id_proyecto = proyecto.Id,
                Id_usuario = idCreador
            });

            return await _proyectoRepository.CrearProyectoAsync(proyecto);
        }
    }
}
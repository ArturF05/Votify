using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class EventoService : IEvento
    {
        private Rol aux = new Rol();
        private Proyecto aux2 = new Proyecto();
        private Votacion aux3 = new Votacion();
        private readonly IEventoRepository eventoRepository;
        public EventoService(IEventoRepository eventoRepository)
        {
            this.eventoRepository = eventoRepository;
        }
        public async Task<List<Evento>?> BuscarEvento(string nombre, Guid Id_Categoria)
        {
            return await eventoRepository.BuscarPorNombreAsync(nombre, Id_Categoria);
        }
        public async Task CrearEvento(string titulo, Guid categoriaId, DateOnly fechaInicio, string descripcion, DateOnly fechaFin)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new Exception("El título es obligatorio");

            if (fechaFin < fechaInicio)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");

            var nuevoEvento = new Evento
            {
                Id = Guid.NewGuid(),
                Nombre = titulo,
                Id_categoria = categoriaId,
                Descripcion = descripcion,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Archivado = false
            };

            await eventoRepository.CrearEventoAsync(nuevoEvento);
        }

        public async Task EditarEvento(Guid eventoId, string titulo, Guid categoriaId, DateOnly fechaInicio, string descripcion, DateOnly fechaFin)
        {
            var evento = await eventoRepository.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            if (string.IsNullOrWhiteSpace(titulo))
                throw new Exception("El título es obligatorio");

            if (fechaFin < fechaInicio)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");

            evento.Nombre = titulo;
            evento.Id_categoria = categoriaId;
            evento.Descripcion = descripcion;
            evento.FechaInicio = fechaInicio;
            evento.FechaFin = fechaFin;

            await eventoRepository.EditarEventoAsync(evento);
        }

        public async Task EliminarEvento(Guid eventoId)
        {
            var evento = await eventoRepository.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            await eventoRepository.EliminarEventoAsync(evento);
        }
        public async Task Archivo(Guid eventoId, bool archivar)
        {
            var evento = await eventoRepository.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            await eventoRepository.ArchivoEventoAsync(evento, archivar);
        }
        public async Task<Evento?> ObtenerEventoPorIdAsync(Guid id)
        {
            return await eventoRepository.ObtenerEventoPorIdAsync(id);
        }
    }
}

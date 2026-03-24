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
        public async Task CrearEvento(string titulo, Categoria categoria, DateOnly fecha_inicio, string descripcion, DateOnly fecha_cierre)
        {
                var nuevoEvento = new Evento
                {
                    Id = Guid.NewGuid(),
                    Nombre = titulo,
                    Id_categoria = categoria.Id,
                    Categoria = categoria,
                    Descripcion = descripcion,
                    FechaInicio = fecha_inicio,
                    FechaFin = fecha_cierre,
                    EventoUsuarioRols = aux,
                    Proyecto = aux2,
                    Votacion = aux3,
                    Archivado = false
                }; 
                await eventoRepository.CrearEventoAsync(nuevoEvento);
         
        }

        public async Task EditarEvento(string titulo, Categoria categoria, DateOnly fecha_inicio, string descripcion, DateOnly fecha_cierre, Evento evento)
        {
            if (titulo == null || categoria == null || fecha_cierre.CompareTo(fecha_inicio) <= 0)
            {

            }
            else
            {
                evento.Nombre = titulo;
                evento.Categoria = categoria;
                evento.Descripcion = descripcion;
                evento.FechaInicio = fecha_inicio;
                evento.FechaFin = fecha_cierre;
                await eventoRepository.EditarEventoAsync(evento);
            }
        }

        public async Task EliminarEvento(Evento evento)
        {
            await eventoRepository.EliminarEventoAsync(evento);
        }
        public async Task Archivo(Evento evento, bool archivar)
        {
            await eventoRepository.ArchivoEventoAsync(evento, archivar);
        }
    }
}

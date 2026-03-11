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
        private Proycto aux2 = new Proycto();
        private Votacion aux3 = new Votacion();
        private readonly IEventoRepository eventoRepository;
        public async Task CrearEvento(string titulo, Categoria categoria, DateTime fecha_inicio, string descripcion, DateTime fecha_cierre)
        {
            if(titulo == null || categoria == null || fecha_inicio == null || fecha_cierre == null)
            {

            }
            else
            {
                var nuevoEvento = new Evento
                {
                    IdEvento = Guid.NewGuid(),
                    Nombre = titulo,
                    Categoria = categoria,
                    Descripcion = descripcion,
                    Inicio = fecha_inicio,
                    Fin = fecha_cierre,
                    Estado = "",
                    IdCategoria = categoria.Id,
                    EventoUsuarioRol = aux,
                    Proyecto = aux2,
                    Votacion = aux3
                }; 
                await eventoRepository.CrearEventoAsync(nuevoEvento);
            }
        }
        public async Task EditarEvento(string titulo, Categoria categoria, DateTime fecha_inicio, string descripcion, DateTime fecha_cierre, Evento evento)
        {
            if (titulo == null || categoria == null || fecha_inicio == null || fecha_cierre == null)
            {

            }
            else
            {
                evento.Nombre = titulo;
                evento.Categoria = categoria;
                evento.Descripcion = descripcion;
                evento.Inicio = fecha_inicio;
                evento.Fin = fecha_cierre;
                await eventoRepository.EditarEventoAsync(evento);
                //evento.Estado = "ns"
            }
        }
        public async Task ArchivarEvento(Evento evento)
        {
            await eventoRepository.ArchivarEventoAsync(evento);
        }
    }
}

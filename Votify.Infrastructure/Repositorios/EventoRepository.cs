using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;


namespace Votify.Infrastructure.Repositorios
{
    public class EventoRepository : IEventoRepository
    {
        private readonly AppDbContext contexto;
        List<Votify.Core.Entities.Evento> eventos = new();
        List<Votify.Core.Entities.Evento> eventosEncontrados = new();

        public EventoRepository(AppDbContext contexto)
        {
            this.contexto = contexto;

        }
        public async Task CrearEventoAsync(Evento evento)
        {
            await contexto.Evento.AddAsync(evento);
            await contexto.SaveChangesAsync();
        }
        public async Task<List<Evento>?> BuscarPorNombreAsync(string nombre, Guid Id_Categoria)
        {
            eventos = await contexto.Evento.Include(e => e.Categoria).Where(e => e.Archivado == false).ToListAsync();
            eventosEncontrados.Clear();
            foreach (var eve in eventos)
            {
                if (eve.Nombre.ToLower().Contains(nombre.ToLower()) && (Id_Categoria == Guid.Empty || eve.Categoria.Id == Id_Categoria))
                {
                    eventosEncontrados.Add(eve);
                }
            }
            return eventosEncontrados;
        }
        public async Task EditarEventoAsync(Evento evento)
        {
            contexto.Update(evento);
            await contexto.SaveChangesAsync();
        }
        public async Task EliminarEventoAsync(Evento evento)
        {
            contexto.Remove(evento);
            await contexto.SaveChangesAsync();
        }
        public async Task ArchivoEventoAsync(Evento evento, bool archivar)
        {
            evento.Archivado = archivar;
            await contexto.SaveChangesAsync();
        }
    }
}

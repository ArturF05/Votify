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
        public async Task CrearEventoAsync(Evento evento)
        {
            await contexto.Eventos.AddAsync(evento);
            await contexto.SaveChangesAsync();
        }
        public async Task<Evento?> BuscarPorNombreAsync(string nombre)
        {
            return await contexto.Eventos.FirstOrDefaultAsync(u => u.Nombre == nombre);
        }
        public async Task EditarEventoAsync(Evento evento)
        {
            contexto.Update(evento);
        }
        public async Task ArchivarEventoAsync(Evento evento)
        {
            //contexto.Eventos.
        }
       
    }
}

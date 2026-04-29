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
    public class EventoUsuarioRolRepository: IEventoUsuarioRolRepository
    {
        private readonly AppDbContext contexto;

        public EventoUsuarioRolRepository(AppDbContext contexto)
        {
            this.contexto = contexto;
        }
        public async Task<List<EventoUsuarioRol>> ObtenerPorEventoYUsuario(Guid eventoId, Guid usuarioId)
        {
            return await contexto.EventoUsuarioRol
                .Include(eur => eur.Rol)
                .Where(eur => eur.EventoId == eventoId && eur.UsuarioId == usuarioId)
                .ToListAsync();
        }
        public async Task<List<EventoUsuarioRol>> ObtenerJuradosPorEventoAsync(Guid eventoId)
        {
            return await contexto.EventoUsuarioRol
                .Include(eur => eur.Rol)
                .Where(eur => eur.EventoId == eventoId && eur.Rol.Nombre == "Jurado")
                .ToListAsync();
        }
        public async Task CrearAsync(EventoUsuarioRol eventoUsuarioRol)
        {
            contexto.EventoUsuarioRol.Add(eventoUsuarioRol);
            await contexto.SaveChangesAsync();
        }
        public async Task EliminarAsync(EventoUsuarioRol eventoUsuarioRol)
        {
            contexto.EventoUsuarioRol.Remove(eventoUsuarioRol);
            await contexto.SaveChangesAsync();
        }
    }
}

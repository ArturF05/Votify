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
    }
}

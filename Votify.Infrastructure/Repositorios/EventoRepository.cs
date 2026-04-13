using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public async Task<Evento> CrearEventoAsync(Evento evento)
        {
            await contexto.Evento.AddAsync(evento);
            await contexto.SaveChangesAsync();
            return evento;
        }
        public async Task<List<Evento>?> BuscarPorNombreAsync(string nombre, Guid Id_Categoria)
        {
            eventos = await contexto.Evento.Include(e => e.Categoria).Where(e => e.Archivado == false).ToListAsync();
            eventosEncontrados.Clear();
            foreach (var eve in eventos)
            {
                if ((string.IsNullOrEmpty(nombre) || eve.Nombre.ToLower().Contains(nombre.ToLower())) && (Id_Categoria == Guid.Empty || eve.Categoria.Id == Id_Categoria))
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
        public async Task ApuntarseEventoAsync(Evento evento, Guid usuarioId, bool apuntar)
        {
            if (apuntar)
            {
                var rolParticipante = await contexto.Role.FirstOrDefaultAsync(r => r.Nombre == "Participante");
                if (rolParticipante == null)
                    throw new Exception("No existe el rol Participante");
                var nuevaRelacion = new EventoUsuarioRol
                {
                    EventoId = evento.Id,
                    UsuarioId = usuarioId,
                    RolId = rolParticipante.Id
                };

                await contexto.EventoUsuarioRol.AddAsync(nuevaRelacion);
            }
            else
            {
                var relacionExistente = await contexto.EventoUsuarioRol
                    .FirstOrDefaultAsync(eur => eur.EventoId == evento.Id && eur.UsuarioId == usuarioId);
                if(relacionExistente != null)
                {
                    contexto.EventoUsuarioRol.Remove(relacionExistente);
                }
            }
            await contexto.SaveChangesAsync();
        }
        public async Task<Evento?> ObtenerEventoPorIdAsync(Guid id)
        {
            return await contexto.Evento.FindAsync(id);
        }
        public async Task<List<Evento>> ObtenerTodos()
        {
            return await contexto.Evento.ToListAsync();
        }
        public async Task<Evento?> ObtenerEventoDashboardAsync(Guid eventoId)
        {
            return await contexto.Evento
                .Include(e => e.Proyectos)
                .FirstOrDefaultAsync(e => e.Id == eventoId);
        }

        public async Task<List<Voto>> ObtenerVotosDeEventoAsync(Guid eventoId)
        {
            var idsProyectos = await contexto.Proyecto
                .Where(p => p.Id_evento == eventoId)
                .Select(p => p.Id)
                .ToListAsync();

            return await contexto.Voto
                .Where(v => idsProyectos.Contains(v.Id_proyecto))
                .ToListAsync();
        }

        public async Task<Votacion?> ObtenerVotacionPorEventoAsync(Guid eventoId)
        {
            return await contexto.Votacion
                .FirstOrDefaultAsync(v => v.Id_evento == eventoId);
        }
        public async Task AsignarOrganizadorAsync(Guid eventoId, Guid usuarioId)
        {
            var rolOrganizador = await contexto.Role
                .FirstOrDefaultAsync(r => r.Nombre == "Organizador");

            if (rolOrganizador == null)
                throw new Exception("No existe el rol Organizador");

            var relacion = new EventoUsuarioRol
            {
                EventoId = eventoId,
                UsuarioId = usuarioId,
                RolId = rolOrganizador.Id
            };

            await contexto.EventoUsuarioRol.AddAsync(relacion);
            await contexto.SaveChangesAsync();
        }
    }
}

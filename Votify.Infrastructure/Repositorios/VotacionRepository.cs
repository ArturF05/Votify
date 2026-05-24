using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;

namespace Votify.Infrastructure.Repositorios
{
    public class VotacionRepository : IVotacionRepository
    {
        private readonly AppDbContext contexto;

        public VotacionRepository(AppDbContext contexto)
        {
            this.contexto = contexto;
        }

        public async Task<Votacion?> ObtenerPorIdAsync(Guid idVotacion)
        {
            return await contexto.Votacion.FirstOrDefaultAsync(v => v.Id == idVotacion);
        }
        public async Task<bool> YaVotoAsync(Guid idVotacion, Guid idVotante)
        {
            return await contexto.Voto.AnyAsync(v => v.Id_votacion == idVotacion && v.Id_votante == idVotante);
        }
        public async Task<bool> YaVotoCategoriaAsync(Guid idVotacion, Guid idVotante, Guid idCategoriaEvento)
        {
            return await contexto.Voto.AnyAsync(v =>
                v.Id_votacion == idVotacion &&
                v.Id_votante == idVotante &&
                v.Id_categoria == idCategoriaEvento);
        }
        public async Task<List<Voto>> ObtenerPorVotacionYUsuarioAsync(Guid votacionId, Guid usuarioId)
        {
            return await contexto.Voto
                .Include(v => v.VotosCriterio)
                .Where(v => v.Id_votacion == votacionId && v.Id_votante == usuarioId)
                .ToListAsync();
        }

        public async Task<List<Votacion>?> BuscarPorNombreAsync(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas)
        {
            IQueryable<Votacion> query = contexto.Votacion;

            query = query.Where(v =>
                (v.FechaFin > DateTime.UtcNow && !verFinalizadas) ||
                (v.FechaFin <= DateTime.UtcNow && verFinalizadas));

            if (Id_evento != Guid.Empty)
                query = query.Where(v => v.Id_evento == Id_evento);

            if (permiteAutoVoto != -1)
            {
                bool permite = permiteAutoVoto == 1;
                query = query.Where(v => v.PermiteAutoVoto == permite);
            }

            if (tipoVotacion != -1)
            {
                var tipo = (TipoVotacion)tipoVotacion;

                query = tipo switch
                {
                    TipoVotacion.Publico => query.Where(v => v is VotacionPublico),
                    TipoVotacion.Jurado => query.Where(v => v is VotacionJurado),
                    TipoVotacion.Mixto => query.Where(v => v is VotacionMixta),
                    _ => query
                };
            }

            return await query.ToListAsync();
        }

        public async Task CrearVotacionAsync(Votacion votacion)
        {
            await contexto.Votacion.AddAsync(votacion);
            await contexto.SaveChangesAsync();
        }

        public async Task EditarVotacionAsync(Votacion votacion)
        {
            contexto.Votacion.Update(votacion);
            await contexto.SaveChangesAsync();
        }

        public async Task ReemplazarVotacionAsync(Votacion original, Votacion nuevaVotacion)
        {
            int tipo = nuevaVotacion.Tipo switch
            {
                TipoVotacion.Publico => 0,
                TipoVotacion.Jurado => 1,
                TipoVotacion.Mixto => 2,
                _ => throw new Exception("Tipo de votación no válido")
            };
            await EliminarVotacionAsync(original);
            await CrearVotacionAsync(nuevaVotacion);
        }

        public async Task EliminarVotacionAsync(Votacion votacion)
        {
            contexto.Votacion.Remove(votacion);
            await contexto.SaveChangesAsync();
        }
        public async Task<Votacion?> ObtenerUltimaPorEventoAsync(Guid eventoId)
        {
            return await contexto.Votacion
                .Include(v => v.DimensionesDeValores)
                .Where(v => v.Id_evento == eventoId)
                .OrderByDescending(v => v.FechaFin)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Votacion>> ObtenerVotacionesEventos(List<Guid> idsEventos)
        {
            return await contexto.Votacion
                .Where(v => idsEventos.Contains(v.Id_evento))
                .ToListAsync();
        }
        public async Task<List<Votacion>> ObtenerCerradasVencidasAsync()
        {
            var ahora = DateTime.UtcNow;
            return await contexto.Votacion
                .Where(v => v.EstadoVotacion == EstadoVotacion.Cerrada && v.FechaInicio <= ahora)
                .ToListAsync();
        }

        public async Task AbrirVotacionAsync(Guid id)
        {
            await contexto.Votacion
                .Where(v => v.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(v => v.EstadoVotacion, EstadoVotacion.Abierta));
        }

        public async Task<List<Voto>> ObtenerVotosVotacion(Guid idVotacion)
        {
            return await contexto.Voto.Where(v => v.Id_votacion == idVotacion).ToListAsync();
        }
    }
}

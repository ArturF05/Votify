using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;

namespace Votify.Infrastructure.Repositorios
{
    public class PremioRepository : IPremioRepository
    {
        private readonly AppDbContext contexto;

        public PremioRepository(AppDbContext contexto)
        {
            this.contexto = contexto;
        }

        public async Task<CategoriaEvento?> ObtenerCategoriaEventoAsync(Guid idCategoriaEvento)
        {
            return await contexto.CategoriaEvento
                .FirstOrDefaultAsync(c => c.Id == idCategoriaEvento);
        }

        public async Task<Premio?> ObtenerPorIdAsync(Guid idPremio)
        {
            return await contexto.Premio
                .Include(p => p.CategoriaEvento)
                .Include(p => p.PremioVotaciones)
                    .ThenInclude(pv => pv.Votacion)
                .FirstOrDefaultAsync(p => p.Id == idPremio);
        }

        public async Task<List<Premio>> ObtenerPremiosAsync(Guid? idCategoriaEvento = null)
        {
            var query = contexto.Premio
                .Include(p => p.CategoriaEvento)
                .Include(p => p.PremioVotaciones)
                    .ThenInclude(pv => pv.Votacion)
                .AsQueryable();

            if (idCategoriaEvento.HasValue)
                query = query.Where(p => p.Id_categoria_evento == idCategoriaEvento.Value);

            return await query
                .OrderBy(p => p.Orden)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExisteNombreEnCategoriaAsync(Guid idCategoriaEvento, string nombre, Guid? excluirPremio = null)
        {
            var nombreNormalizado = nombre.Trim().ToLower();

            return await contexto.Premio.AnyAsync(p =>
                p.Id_categoria_evento == idCategoriaEvento &&
                p.Nombre.ToLower() == nombreNormalizado &&
                (!excluirPremio.HasValue || p.Id != excluirPremio.Value));
        }

        public async Task<Votacion?> ObtenerVotacionAsync(Guid idVotacion)
        {
            return await contexto.Votacion
                .FirstOrDefaultAsync(v => v.Id == idVotacion);
        }

        public async Task<bool> VotacionIncluyeCategoriaAsync(Guid idVotacion, Guid idCategoriaEvento)
        {
            return await contexto.DimensionesDeValor.AnyAsync(d =>
                d.Id_votacion == idVotacion &&
                d.Id_categoria_evento == idCategoriaEvento);
        }

        public async Task CrearAsync(Premio premio)
        {
            await contexto.Premio.AddAsync(premio);
            await contexto.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Premio premio)
        {
            contexto.Premio.Update(premio);
            await contexto.SaveChangesAsync();
        }

        public async Task EliminarAsync(Premio premio)
        {
            contexto.Premio.Remove(premio);
            await contexto.SaveChangesAsync();
        }

        public async Task<List<PremioVotacion>> SincronizarVinculosAsync(Guid idPremio, IEnumerable<Guid> idsVotaciones)
        {
            var idsNormalizados = idsVotaciones
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var vinculosActuales = await contexto.PremioVotacion
                .Where(pv => pv.Id_premio == idPremio)
                .ToListAsync();

            var vinculosAEliminar = vinculosActuales
                .Where(pv => !idsNormalizados.Contains(pv.Id_votacion))
                .ToList();

            contexto.PremioVotacion.RemoveRange(vinculosAEliminar);

            var idsActuales = vinculosActuales
                .Select(pv => pv.Id_votacion)
                .ToHashSet();

            foreach (var idVotacion in idsNormalizados.Where(id => !idsActuales.Contains(id)))
            {
                await contexto.PremioVotacion.AddAsync(new PremioVotacion
                {
                    Id_premio = idPremio,
                    Id_votacion = idVotacion
                });
            }

            await contexto.SaveChangesAsync();

            return await contexto.PremioVotacion
                .Include(pv => pv.Votacion)
                .Where(pv => pv.Id_premio == idPremio)
                .ToListAsync();
        }

        public async Task<List<Premio>> ObtenerPremiosPorVotacionAsync(Guid idVotacion, Guid? idCategoriaEvento = null)
        {
            var query = contexto.Premio
                .Include(p => p.CategoriaEvento)
                .Include(p => p.PremioVotaciones)
                    .ThenInclude(pv => pv.Votacion)
                .Where(p => p.PremioVotaciones.Any(pv => pv.Id_votacion == idVotacion));

            if (idCategoriaEvento.HasValue)
                query = query.Where(p => p.Id_categoria_evento == idCategoriaEvento.Value);

            return await query
                .OrderBy(p => p.Orden)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
        }
    }
}

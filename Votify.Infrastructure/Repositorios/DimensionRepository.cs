using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Votify.Infrastructure.Repositorios
{
    public class DimensionRepository : IDimensionRepository
    {
        private readonly AppDbContext contexto;
        public DimensionRepository(AppDbContext contexto)
        {
            this.contexto = contexto;
        }
        public async Task<List<DimensionesDeValor?>> ObtenerCriterioPorVotacionAsync(Guid id)
        {
            return await contexto.DimensionesDeValor
                .Where(d => d.Id_votacion == id)
                .ToListAsync();
        }
        public async Task CrearCriterioAsync(DimensionesDeValor nuevoCriterio)
        {
            contexto.Add(nuevoCriterio);
            await contexto.SaveChangesAsync();
        }
        public async Task EliminarCriterioAsync(DimensionesDeValor criterio)
        {
            contexto.Remove(criterio);
            await contexto.SaveChangesAsync();
        }
        public async Task EditarCriterioAsync(DimensionesDeValor criterio)
        {
            contexto.Update(criterio);
            await contexto.SaveChangesAsync();
        }
    }
}

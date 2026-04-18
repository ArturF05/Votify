using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IDimensionRepository
    {
        public Task<List<DimensionesDeValor?>> ObtenerCriterioPorVotacionAsync(Guid id);
        public Task CrearCriterioAsync(DimensionesDeValor nuevoCriterio);
        public Task EliminarCriterioAsync(DimensionesDeValor criterio);
    }
}

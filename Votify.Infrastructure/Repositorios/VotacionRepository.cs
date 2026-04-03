using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}

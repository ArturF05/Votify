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
    public class VotoRepository : IVotoRepository
    {
        private readonly AppDbContext contexto;

        public VotoRepository(AppDbContext contexto)
        {
            this.contexto = contexto;
        }

        public async Task<bool> UsuarioYaVotoAsync(Guid idVotacion, Guid idVotante)
        {
            return await contexto.Voto.AnyAsync(v => v.Id_votacion == idVotacion && v.Id_votante == idVotante);
        }

        public async Task GuardarVotosAsync(List<Voto> votos)
        {
            await contexto.Voto.AddRangeAsync(votos);
            await contexto.SaveChangesAsync();
        }
    }
}

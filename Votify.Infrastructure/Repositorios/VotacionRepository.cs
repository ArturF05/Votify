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
        List<Votify.Core.Entities.Votacion> votaciones = new();
        List<Votify.Core.Entities.Votacion> votacionesEncontradas = new();


        public VotacionRepository(AppDbContext contexto)
        {
            this.contexto = contexto;
        }

        public async Task<Votacion?> ObtenerPorIdAsync(Guid idVotacion)
        {
            return await contexto.Votacion.FirstOrDefaultAsync(v => v.Id == idVotacion);
        }

        public async Task<List<Votacion>?> BuscarPorNombreAsync(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas)
        {
            votaciones = await contexto.Votacion.Include(v => v.Evento).ToListAsync();
            votaciones = votaciones.Where(vot => (vot.FechaFin > DateTime.UtcNow && !verFinalizadas)|| (vot.FechaFin <= DateTime.UtcNow && verFinalizadas)).ToList();
            votacionesEncontradas.Clear();
            foreach (var vot in votaciones)
            {
                if (tipoVotacion!=null && (tipoVotacion == -1 || vot.TipoVotacion == (TipoVotacion)tipoVotacion) && Id_evento != null && (Id_evento == Guid.Empty || vot.Evento.Id == Id_evento) && (permiteAutoVoto == -1 || vot.PermiteAutoVoto == (permiteAutoVoto == 1)))
                {
                    votacionesEncontradas.Add(vot);
                }
            }
            return votacionesEncontradas;
        }
        public async Task CrearVotacionAsync(Votacion votacion)
        {
            await contexto.Votacion.AddAsync(votacion);

        }
        public async Task EditarVotacionAsync(Votacion votacion)
        {
            contexto.Update(votacion);
            await contexto.SaveChangesAsync();
        }
        public async Task EliminarVotacionAsync(Votacion votacion)
        {
            contexto.Remove(votacion);
            await contexto.SaveChangesAsync();
        }
    }
}

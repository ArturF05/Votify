using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;

namespace Votify.Infrastructure.Repositorios
{
    public class ProyectoRepository : IProyectoRepository
    {
        private readonly AppDbContext contexto;
        public ProyectoRepository(AppDbContext context)
        {
            contexto = context;
        }
        public async Task<bool> CrearProyectoAsync(Proyecto proyecto)
        {
            await contexto.Proyecto.AddAsync(proyecto);
            await contexto.SaveChangesAsync();
            return true;
        }
        public async Task<List<Proyecto>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            return await contexto.Proyecto
                .Where(p => p.Id_creador == usuarioId)
                .Include(p => p.Materiales) 
                .ToListAsync();
        }
        public async Task<List<Proyecto>> ObtenerPorIdAsync(List<Guid> idsProyectos)
        {
            return await contexto.Proyecto.Include(p => p.ProyectoParticipantes).Where(p => idsProyectos.Contains(p.Id)).ToListAsync();
        }
        public async Task<List<Proyecto>> ObtenerPorEventoIdAsync(Guid eventoId)
        {
            return await contexto.Proyecto
                .Where(p => p.Id_evento == eventoId)
                .Include(p => p.Materiales)
                .Include(p => p.ProyectoParticipantes)
                .ToListAsync();
        }

        public async Task<List<CriterioDashboardDTO>> ObtenerComentariosPorCriterioDeProyecto(Guid idProyecto)
        {
            var votosIds = await contexto.Voto
                .Where(v => v.Id_proyecto == idProyecto)
                .Select(v => v.Id)
                .ToListAsync();

            return await contexto.DimensionesDeValor
                .Select(c => new CriterioDashboardDTO
                {
                    Nombre = c.Nombre,

                    Comentarios = contexto.VotoCriterio
                        .Where(vc =>
                            vc.Id_criterio == c.Id &&
                            votosIds.Contains(vc.Id_voto) &&
                            vc.Comentario != null &&
                            vc.Comentario != "")
                        .Select(vc => vc.Comentario!)
                        .ToList()
                })
                .Where(c => c.Comentarios.Any())
                .ToListAsync();
        }
    }
}

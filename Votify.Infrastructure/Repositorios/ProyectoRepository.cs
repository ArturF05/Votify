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

        public async Task<List<Proyecto>> ObtenerPorIdAsync(List<Guid> idsProyectos)
        {
            return await contexto.Proyecto.Include(p => p.ProyectoParticipantes).Where(p => idsProyectos.Contains(p.Id)).ToListAsync();
        }
    }
}

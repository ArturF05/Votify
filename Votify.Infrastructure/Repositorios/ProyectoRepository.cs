using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;

namespace Votify.Infrastructure.Repositorios
{
    public class ProyectoRepository : IProyectoRepository
    {
        private readonly AppDbContext _context;
        public ProyectoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CrearProyectoAsync(Proyecto proyecto)
        {
            
                    
            await _context.Proyecto.AddAsync(proyecto);
           await _context.SaveChangesAsync();

            return true;
        }
    }
}

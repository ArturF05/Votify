using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;

namespace Votify.Infrastructure.Repositorios
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AppDbContext _context;

        public MaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Material>> ObtenerPorUsuarioAsync(Guid usuarioId)
        {
            // Buscamos los materiales cuyo proyecto pertenezca al usuario
            return await _context.Material
                .Include(m => m.Proyecto) 
               .Where(m => m.Proyecto.Id_creador == usuarioId)
                .ToListAsync();
        }

        public async Task<Material?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Material.FindAsync(id);
        }

        public async Task CrearAsync(Material material)
        {
            await _context.Material.AddAsync(material);
        }

        public async Task EliminarAsync(Material material)
        {
            _context.Material.Remove(material);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

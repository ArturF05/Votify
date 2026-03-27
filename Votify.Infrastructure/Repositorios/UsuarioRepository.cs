using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Votify.Infrastructure.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorCredencialesAsync(string usuario, string password)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Nombre.Trim()== usuario.Trim() && u.Password.Trim() == password.Trim());
        }

        public async Task<Usuario?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Nombre == nombre);
        }

        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
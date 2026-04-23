using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorCredencialesAsync(string usuario, string password);
        Task<Usuario?> ObtenerPorNombreAsync(string nombre);
        Task CrearUsuarioAsync(Usuario usuario);
        Task<Usuario?> GetByCredencialesAsync(string usuario, string password);
        Task<Usuario?> ObtenerPorIdAsync(Guid idUsuario);
    }
}

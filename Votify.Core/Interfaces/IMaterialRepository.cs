using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IMaterialRepository
    {
        Task<List<Material>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<Material?> ObtenerPorIdAsync(Guid id);
        Task CrearAsync(Material material);
        Task EliminarAsync(Material material);
        Task GuardarCambiosAsync();
    }
}
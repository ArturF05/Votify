using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IPremio
    {
        Task<Premio> CrearPremioAsync(Guid idCategoriaEvento, string nombre, string? descripcion, int orden, IEnumerable<Guid>? idsVotaciones = null, string? imagenUrl = null, int puesto = 1);
        Task<Premio> EditarPremioAsync(Guid idPremio, string nombre, string? descripcion, int orden, string? imagenUrl, int puesto = 1, IEnumerable<Guid>? idsVotaciones = null);
        Task<List<Premio>> ObtenerPremiosAsync(Guid? idCategoriaEvento = null);
        Task<Premio?> ObtenerPorIdAsync(Guid idPremio);
        Task<List<PremioVotacion>> ActualizarVotacionesPremioAsync(Guid idPremio, IEnumerable<Guid> idsVotaciones);
        Task<List<Premio>> ObtenerPremiosPorVotacionAsync(Guid idVotacion, Guid? idCategoriaEvento = null);
        Task EliminarPremioAsync(Guid idPremio);
    }
}

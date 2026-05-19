using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IPremioRepository
    {
        Task<CategoriaEvento?> ObtenerCategoriaEventoAsync(Guid idCategoriaEvento);
        Task<Premio?> ObtenerPorIdAsync(Guid idPremio);
        Task<List<Premio>> ObtenerPremiosAsync(Guid? idCategoriaEvento = null);
        Task<bool> ExisteNombreEnCategoriaAsync(Guid idCategoriaEvento, string nombre, Guid? excluirPremio = null);
        Task<Votacion?> ObtenerVotacionAsync(Guid idVotacion);
        Task<bool> VotacionIncluyeCategoriaAsync(Guid idVotacion, Guid idCategoriaEvento);
        Task CrearAsync(Premio premio);
        Task ActualizarAsync(Premio premio);
        Task EliminarAsync(Premio premio);
        Task<List<PremioVotacion>> SincronizarVinculosAsync(Guid idPremio, IEnumerable<Guid> idsVotaciones);
        Task<List<Premio>> ObtenerPremiosPorVotacionAsync(Guid idVotacion, Guid? idCategoriaEvento = null);
    }
}

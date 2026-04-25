using Votify.Core.DTOs;

namespace Votify.Core.Interfaces
{
    public interface IMaterialService
    {
        Task<List<ProyectoConMaterialesDto>> ObtenerProyectosUsuario(Guid usuarioId);
        Task GuardarMateriales(Guid proyectoId, List<MaterialDto> materiales);
        Task EliminarMaterial(Guid materialId);
    }
}
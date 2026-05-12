namespace Votify.API.DTOs
{
    public class CategoriaEventoConCriteriosDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public List<Votify.Core.DTOs.CrearCriterioDTO> Criterios { get; set; } = new();
    }

    public class CrearCategoriaEventoDTO
    {
        public string Nombre { get; set; }
    }

    public class CrearCriterioCategoriaDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Peso { get; set; }
    }
}

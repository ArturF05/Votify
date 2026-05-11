

namespace Votify.Core.DTOs
{
    public class CrearCriterioDTO
    {
        public Guid Id { get; set; }
        public Guid IdCategoriaEvento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Peso { get; set; }
    }
}

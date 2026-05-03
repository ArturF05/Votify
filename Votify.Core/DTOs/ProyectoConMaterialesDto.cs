namespace Votify.Core.DTOs
{
    public class ProyectoConMaterialesDto
    {
        public Guid ProyectoId { get; set; }
        public string Titulo { get; set; } = string.Empty; 
        public List<MaterialDto> Materiales { get; set; } = new();
    }
}
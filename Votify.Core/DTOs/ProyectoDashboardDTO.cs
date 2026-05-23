namespace Votify.Core.DTOs
{
    public class ProyectoDashboardDTO
    {
        public Guid ProyectoId { get; set; }
        public string TituloProyecto { get; set; } = "";
        public decimal PuntuacionTotal { get; set; }
        public Guid? IdCategoriaEvento { get; set; }
        public string NombreCategoriaEvento { get; set; } = string.Empty;
        public string CreadorNombre { get; set; } = "Creador desconocido";
        public List<MaterialDto> Materiales { get; set; } = new();
        public List<string> Comentarios { get; set; } = new();
        public List<CriterioDashboardDTO> Criterios { get; set; } = new();
    }
}

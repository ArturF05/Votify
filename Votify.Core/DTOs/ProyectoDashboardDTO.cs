namespace Votify.Core.DTOs
{
    public class ProyectoDashboardDTO
    {
        public Guid ProyectoId { get; set; }
        public string TituloProyecto { get; set; } = "";
        public decimal PuntuacionTotal { get; set; }
        public List<string> Comentarios { get; set; } = new();
        public List<CriterioDashboardDTO> Criterios { get; set; } = new();
    }
}

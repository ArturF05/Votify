namespace Votify.Core.DTOs
{
    public class DashboardEventoDTO
    {
        public Guid EventoId { get; set; }
        public string NombreEvento { get; set; } = "";
        public Guid ProyectoGanadorId { get; set; }
        public string TituloProyectoGanador { get; set; } = "";
        public decimal PuntuacionProyectoGanador { get; set; }
        public List<CriterioDashboardDTO> Criterios { get; set; } = new();
        public List<ProyectoDashboardDTO> Proyectos { get; set; } = new();
    }
}

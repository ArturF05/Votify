namespace Votify.Core.DTOs
{
    public class CriterioDashboardDTO
    {
        public Guid CriterioId { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Peso { get; set; }
        public decimal PuntuacionMedia { get; set; }
        public List<string> Comentarios { get; set; } = new();
    }
}
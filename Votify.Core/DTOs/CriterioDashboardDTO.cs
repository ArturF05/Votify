namespace Votify.Core.DTOs
{
    public class CriterioDashboardDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public int Peso { get; set; }
        public decimal? ValorPromedio { get; set; }

    }
}

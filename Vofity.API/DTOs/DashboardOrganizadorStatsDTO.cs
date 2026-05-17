namespace Votify.API.DTOs
{
    public class DashboardOrganizadorStatsDTO
    {
        public int EventosActivos { get; set; }
        public int ProyectosTotales { get; set; }
        public int Participantes { get; set; }
        public int VotosEmitidos { get; set; }
    }
}

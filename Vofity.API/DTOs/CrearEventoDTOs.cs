using Votify.Core.Entities;

namespace Votify.API.DTOs
{
    public class CrearEventoDTOs
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public Guid Id_categoria { get; set; }
        public bool Archivado { get; set; }
        public string Tipo { get; set; }
        public List<CrearVotacionDTOs> dTOsVotacion { get; set; }
        public Guid UsuarioCreadorId { get; set; }
    }
}

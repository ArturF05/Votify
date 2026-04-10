using Votify.Core.Entities;

namespace Votify.API.DTOs
{
    public class CrearEventoDTOs
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public Guid Id_categoria { get; set; }
        public bool Archivado { get; set; }
        public TipoVotacion TipoVotacion { get; set; }
    }
}

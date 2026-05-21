using System;

namespace Votify.Core.DTOs
{
    public class VotacionDetalleDTO
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public Guid? IdCategoriaEvento { get; set; }

        public string NombreCategoriaEvento { get; set; } = string.Empty;
    }
}
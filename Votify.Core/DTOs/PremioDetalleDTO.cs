using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class PremioDetalleDTO
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        public int Puesto { get; set; }

        public Guid? IdVotacion { get; set; }

        public string NombreVotacion { get; set; } = string.Empty;

        public bool TieneGanador { get; set; }

        public Guid? IdProyectoGanador { get; set; }

        public string? NombreProyectoGanador { get; set; }

        public decimal? PuntuacionGanador { get; set; }

        public bool VotacionFinalizada { get; set; }

        public string EstadoPremio { get; set; } = string.Empty;

        public Guid? IdCategoriaEvento { get; set; }

        public string NombreCategoriaEvento { get; set; } = string.Empty;
    }
}

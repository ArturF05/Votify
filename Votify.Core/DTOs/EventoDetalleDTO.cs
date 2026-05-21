using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class EventoDetalleDTO
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public string? ImagenUrl { get; set; }

        public string TipoEvento { get; set; } = string.Empty;

        public string Organizador { get; set; } = string.Empty;

        public List<ProyectoDashboardDTO> Proyectos { get; set; } = new();

        public List<CategoriaEventoResumenDTO> Categorias { get; set; } = new();

        public List<VotacionDetalleDTO> Votaciones { get; set; } = new();
    }
}

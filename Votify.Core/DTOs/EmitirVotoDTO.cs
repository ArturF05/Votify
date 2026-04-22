using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class VotoDetalleDTO
    {
        public Guid IdProyecto { get; set; }

        public string Comentario { get; set; }
        public List<PuntuacionCriterioDTO> Puntuaciones { get; set; } = new();
    }
    public class EmitirVotoDTO
    {
        public Guid IdVotante { get; set; }
        public List<Guid> IdsProyectos { get; set; } = new();
        public List<VotoDetalleDTO> Votos { get; set; }
    }
}

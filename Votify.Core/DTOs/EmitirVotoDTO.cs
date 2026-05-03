using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.DTOs
{
    public class VotoDetalleDTO
    {
        public Guid IdProyecto { get; set; }

        public List<PuntuacionCriterioDTO> Puntuaciones { get; set; } = new();
    }
    public class EmitirVotoDTO
    {
        public Guid IdVotante { get; set; }
        public Guid IdEvento { get; set; }
        public List<VotoDetalleDTO> Votos { get; set; }
    }
}

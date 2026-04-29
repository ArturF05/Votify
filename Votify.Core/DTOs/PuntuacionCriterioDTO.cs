using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class PuntuacionCriterioDTO
    {
        public Guid IdCriterio { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = "";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class PuntuacionCriterioDTO
    {
        public Guid CriterioId { get; set; }
        public string NombreCriterio { get; set; } = "";
        public decimal Valor { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class CrearProyectoDto
    {
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}

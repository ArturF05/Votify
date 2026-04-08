using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Votacion
{
    public Guid Id { get; set; }
    public Boolean PermiteAutoVoto { get; set; }
    public Guid Id_evento { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DimensionesDeValor? DimensionesDeValores { get; set; }
    public Resultado? Resultados { get; set; }
   
}

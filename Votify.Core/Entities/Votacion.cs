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
    public Guid id_evento { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public DimensionesDeValor DimensionesDeValors { get; set; }
    public Resultado Resultados { get; set; }
}

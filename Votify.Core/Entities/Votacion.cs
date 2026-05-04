using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public abstract class Votacion
{
    public Guid Id { get; set; }
    public abstract TipoVotacion Tipo { get; }
    public Boolean PermiteAutoVoto { get; set; }
    public Guid Id_evento { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public List<DimensionesDeValor> DimensionesDeValores { get; set; } = new();
    public Resultado? Resultados { get; set; }
    public int NumProy { get; set; }
    public bool PermiteComentarios { get; set; }
    public bool ComentariosObligatorios { get; set; }
    public bool VerRes { get; set; }
    public EstadoVotacion EstadoVotacion { get; set; } = EstadoVotacion.Cerrada;

}

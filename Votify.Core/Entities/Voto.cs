using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Voto
{
    public Guid Id { get; set; }
    [Column("Id_votacion")]
    public Guid Id_votacion { get; set; }
    [Column("Id_votante")]
    public Guid Id_votante { get; set; }
    [Column("Id_categoria")]
    public Guid? Id_categoria { get; set; }
    [Column("Id_proyecto")]
    public Guid Id_proyecto { get; set; }
    public decimal Puntuacion { get; set; }
    public DateOnly Fecha_voto { get; set; }
    [ForeignKey(nameof(Id_categoria))]
    public Categoria? Categoria { get; set; }
    [ForeignKey(nameof(Id_votante))]
    public Usuario Usuario { get; set; }
    [ForeignKey(nameof(Id_votacion))]
    public Votacion Votacion { get; set; }
}

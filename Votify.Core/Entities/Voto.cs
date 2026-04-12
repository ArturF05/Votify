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
    public Guid Id_votacion { get; set; }
    public Guid Id_votante { get; set; }
    public Guid? Id_categoria { get; set; }
    public Guid Id_proyecto { get; set; }
    public decimal Puntuacion { get; set; }
    public DateTime Fecha_voto { get; set; }
    [ForeignKey(nameof(Id_categoria))]
    public Categoria? Categoria { get; set; }
    [ForeignKey(nameof(Id_votante))]
    public Usuario Usuario { get; set; }
    [ForeignKey(nameof(Id_votacion))]
    public Votacion Votacion { get; set; }
    public string? Comentario { get; set; }
}

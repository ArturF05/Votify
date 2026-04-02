using System;
using System.Collections.Generic;
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
    public DateOnly Fecha_voto { get; set; }
    public Categoria Categoria { get; set; }
    public Usuario Usuario { get; set; }
    public Votacion Votacion { get; set; }
}

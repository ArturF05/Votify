using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class DimensionesDeValor
{
    public Guid Id { get; set; }
    public Guid Id_votacion {  get; set; }
    public String Nombre { get; set; }
    public String Descripcion { get; set; }
    public decimal Peso {  get; set; }
    public Votacion? Votacion { get; set; }
}

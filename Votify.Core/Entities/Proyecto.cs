using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Proyecto
{
    public Guid Id { get; set; }
    public String Titulo { get; set; }
    public Guid Id_evento { get; set; }
    public Evento Evento { get; set; }
    public String Descripcion { get; set; }
}

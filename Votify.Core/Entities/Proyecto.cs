using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Proyecto
{
    public Guid Id { get; set; }
    public String Titulo { get; set; }
    [ForeignKey(nameof(Evento))]
    public Guid Id_evento { get; set; }
    public Evento Evento { get; set; }
    public String Descripcion { get; set; }
    public Guid Id_creador { get; set; }
    public List<ProyectoParticipantes> ProyectoParticipantes { get; set; }
    public ICollection<Material> Materiales { get; set; }


}

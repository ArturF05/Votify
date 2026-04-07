using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class EventoUsuarioRol
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }
    public Evento Evento { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    public Guid RolId { get; set; }
    public Rol Rol { get; set; }
}

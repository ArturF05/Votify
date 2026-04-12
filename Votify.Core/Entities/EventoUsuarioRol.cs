using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class EventoUsuarioRol
{
    [NotMapped]
    public Guid Id { get; set; }
    [Column("id_evento")]
    public Guid EventoId { get; set; }
    public Evento Evento { get; set; }
    [Column("id_usuario")]
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    [Column("id_rol")]
    public Guid RolId { get; set; }
    public Rol Rol { get; set; }
}

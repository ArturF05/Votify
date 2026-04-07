using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Password { get; set; } = "";

    [Column("Id_rol")]
    public Guid Id_rol { get; set; }

    [ForeignKey(nameof(Id_rol))]
    public Rol Rol { get; set; }
}
    
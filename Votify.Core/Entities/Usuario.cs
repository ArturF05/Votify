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
    public Guid id_Rol { get; set; }

    [ForeignKey("id_Rol")]
    public Rol Rol { get; set; }
}
    
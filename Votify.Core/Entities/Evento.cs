using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Evento
{
    public Guid IdEvento { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime Inicio {  get; set; }
    public DateTime Fin {  get; set; }
    public string Estado { get; set; }
    public Guid IdCategoria { get; set; }
    public Categoria Categoria { get; set; }
    public Rol EventoUsuarioRol { get; set; }
    public Proycto Proyecto { get; set; }
    public Votacion Votacion { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Material
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty; // Ej: "Fachada principal"
    public string Tipo { get; set; } = "Imagen";      // "Imagen", "PDF", etc.

    
    //la URL que te de Azure
    public string Url { get; set; } = string.Empty;

  
    public Guid ProyectoId { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}

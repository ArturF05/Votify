using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Categoria
{
    [Key]
    public Guid Id { get; set; }
    public string Nombre { get; set; } 
}

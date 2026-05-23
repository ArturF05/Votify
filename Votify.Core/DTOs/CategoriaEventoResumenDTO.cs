using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class CategoriaEventoResumenDTO
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}

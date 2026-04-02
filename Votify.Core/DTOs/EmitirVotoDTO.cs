using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class EmitirVotoDTO
    {
        public Guid IdVotante { get; set; }
        public List<Guid> IdsProyectos { get; set; } = new();
    }
}

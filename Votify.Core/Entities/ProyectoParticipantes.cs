using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities
{
    public class ProyectoParticipantes
    {
        public Guid Id_proyecto { get; set; }
        public Guid Id_usuario { get; set; }
        public Proyecto Proyecto { get; set; }
        public Usuario Usuario { get; set; }

    }
}

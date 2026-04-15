using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities
{
    public class VotacionJurado : Votacion
    {
        public override TipoVotacion Tipo => TipoVotacion.Jurado;
    }
}

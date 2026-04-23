using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IVotoRepository
    {
        Task GuardarVotosAsync(List<Voto> votos);
    }
}

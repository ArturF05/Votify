using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IVotacion
    {
        Task<bool> RegistrarVotoAsync(Guid idVotacion, EmitirVotoDTO dto);
    }
}

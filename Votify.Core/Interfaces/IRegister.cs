using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Interfaces
{
    public interface IRegister
    {
        Task<bool> RegistrarUsuarioAsync(string usuario, string password);
    }
}


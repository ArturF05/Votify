using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Interfaces
{
    public interface ILogin
    {
        Task<bool> ValidarUsuarioAsync(string usuario, string password);
    }
}

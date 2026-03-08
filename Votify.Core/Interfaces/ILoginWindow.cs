using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Votify.Core.Interfaces.ILoginWindow;

namespace Votify.Core.Interfaces
{
    public interface ILoginWindow
    { }
        public class LoginWindow : ILoginWindow
        {
            public await void ValidarUsuario(string usuario, string password) { }
        }

    
}

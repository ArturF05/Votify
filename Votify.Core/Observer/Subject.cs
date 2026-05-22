using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Observer
{
    public interface Subject
    {
        void suscribir(Observer observer);
        void desuscribir(Observer observer);
        void notificar(String mensaje);
    }
}

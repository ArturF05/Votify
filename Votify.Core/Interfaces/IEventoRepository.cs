using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IEventoRepository
    {
        Task CrearEventoAsync(Evento evento);
        Task<Evento?> BuscarPorNombreAsync(string nombre);
        Task EditarEventoAsync(Evento evento);
        Task ArchivarEventoAsync(Evento evento);



    }
}

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
        Task<List<Evento>?> BuscarPorNombreAsync(string nombre, Guid Id_Categoria);
        Task EditarEventoAsync(Evento evento);
        Task EliminarEventoAsync(Evento evento);
        Task ArchivoEventoAsync(Evento evento, bool archivar);


    }
}

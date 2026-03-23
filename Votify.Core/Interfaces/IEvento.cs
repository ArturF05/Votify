using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Interfaces
{
    public interface IEvento
    {
        Task CrearEvento(string titulo, Categoria categoria, DateOnly fecha_inicio, string descripcion, DateOnly fecha_cierre);
        Task EditarEvento(string titulo, Categoria categoria, DateOnly fecha_inicio, string descripcion, DateOnly fecha_cierre, Evento evento);
        Task EliminarEvento(Evento evento);
        Task<List<Evento>?> BuscarEvento(string nombre, Guid Id_Categoria);
        Task Archivo(Evento evento, bool archivar);
    }
}

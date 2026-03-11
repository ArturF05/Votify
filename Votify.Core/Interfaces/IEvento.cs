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
        Task CrearEvento(string titulo, Categoria categoria, DateTime fecha_inicio, string descripcion, DateTime fecha_cierre);
        Task EditarEvento(string titulo, Categoria categoria, DateTime fecha_inicio, string descripcion, DateTime fecha_cierre, Evento evento);
        Task ArchivarEvento(Evento evento);
    }
}

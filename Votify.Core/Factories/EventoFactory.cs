using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Factories
{
    //Fábrica parametrizada para crear diferentes tipos de eventos (Hackathon, Concurso, etc.) según el tipo especificado.
    public abstract class EventoFactory
    {
        public abstract Evento CreateEvento(string tipo, string nombre, string descripcion, DateTime fechaInicio, DateTime fechaFin, Guid idCategoria);
    }
}

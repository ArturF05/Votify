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
        public abstract Evento CreateEvento(string tipo, TipoVotacion tipoVotacion, string nombre, string descripcion, DateOnly fechaInicio, DateOnly fechaFin, Guid idCategoria);
    }
}

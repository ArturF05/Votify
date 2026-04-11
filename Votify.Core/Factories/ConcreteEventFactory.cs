using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Votify.Core.Factories
{
    public class ConcreteEventFactory : EventoFactory
    {
        public override Evento CreateEvento(string tipo,TipoVotacion tipoVotacion, string nombre, string descripcion, DateOnly fechaInicio, DateOnly fechaFin, Guid idCategoria)
        {
            switch (tipo)
            {
                case "Hackathon":
                    return new EventoHackathon
                    {
                        Id = Guid.NewGuid(),
                        Nombre = nombre,
                        Descripcion = descripcion,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        Id_categoria = idCategoria,
                        TipoVotacion = tipoVotacion,
                        Archivado = false
                    };

                case "Concurso":
                    return new EventoConcurso
                    {
                        Id = Guid.NewGuid(),
                        Nombre = nombre,
                        Descripcion = descripcion,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin,
                        Id_categoria = idCategoria,
                        TipoVotacion = tipoVotacion,
                        Archivado = false
                    };

                default:
                    throw new ArgumentException("Tipo de evento no reconocido");
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Votify.Core.Factories
{
    internal class ConcreteVotacionFactory : VotacionFactory
    {
        public override Votacion CrearVotacion(DateTime fechaInicio, DateTime fechaFin, bool permiteAutoVoto, Guid Id_evento, TipoVotacion tipoVotacion,int NumProy, bool permiteComentarios, bool comentariosObligatorios, EstadoVotacion estadoVotacion)
         {
            switch (tipoVotacion)
            {
                case TipoVotacion.Publico:
                    return new VotacionPublico
                    {
                        Id = Guid.NewGuid(),
                        PermiteAutoVoto = permiteAutoVoto,
                        Id_evento = Id_evento,
                        FechaInicio = fechaInicio.ToUniversalTime(),
                        FechaFin = fechaFin.ToUniversalTime(),
                        NumProy= NumProy,
                        PermiteComentarios = permiteComentarios,
                        ComentariosObligatorios = comentariosObligatorios,
                        EstadoVotacion=estadoVotacion

                    };

                case TipoVotacion.Jurado:
                    return new VotacionJurado
                    {
                        Id = Guid.NewGuid(),
                        PermiteAutoVoto = permiteAutoVoto,
                        Id_evento = Id_evento,
                        FechaInicio = fechaInicio.ToUniversalTime(),
                        FechaFin = fechaFin.ToUniversalTime(),
                        NumProy = NumProy,
                        PermiteComentarios = permiteComentarios,
                        ComentariosObligatorios = comentariosObligatorios,
                        EstadoVotacion=estadoVotacion
                    };

                case TipoVotacion.Mixto:
                    return new VotacionMixta
                    {
                        Id = Guid.NewGuid(),
                        PermiteAutoVoto = permiteAutoVoto,
                        Id_evento = Id_evento,
                        FechaInicio = fechaInicio.ToUniversalTime(),
                        FechaFin = fechaFin.ToUniversalTime(),
                        NumProy = NumProy,
                        PermiteComentarios = permiteComentarios,
                        ComentariosObligatorios = comentariosObligatorios,
                        EstadoVotacion=estadoVotacion

                    };
                default:
                    throw new ArgumentException("Tipo de votación no reconocido");
            }

        }
    }
}



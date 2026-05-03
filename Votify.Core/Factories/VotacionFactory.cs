using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Factories
{
    internal abstract class VotacionFactory
    {
        public abstract Votacion CrearVotacion(DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy,bool verRes, EstadoVotacion estadoVotacion);
        public virtual Votacion RecrearVotacion(Guid id,DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy,bool verRes, EstadoVotacion estadoVotacion)
        {
            var votacion = CrearVotacion(
                fechaInicio,
                fechaFin,
                permiteAutoVoto,
                Id_evento,
                tipoVotacion,
                NumProy,
                verRes,
                estadoVotacion
            );

            votacion.Id = id;
            return votacion;
        }
    }
}

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
        public abstract Votacion CrearVotacion(DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy);
        public virtual Votacion RecrearVotacion(Guid id,DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy)
        {
            var votacion = CrearVotacion(
                fechaInicio,
                fechaFin,
                permiteAutoVoto,
                Id_evento,
                tipoVotacion,
                NumProy
            );

            votacion.Id = id;
            return votacion;
        }
    }
}

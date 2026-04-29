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
        public abstract Votacion CrearVotacion(DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy, bool permiteComentarios, bool comentariosObligatorios);
        public virtual Votacion RecrearVotacion(Guid id,DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy, bool permiteComentarios, bool comentariosObligatorios)
        {
            var votacion = CrearVotacion(
                fechaInicio,
                fechaFin,
                permiteAutoVoto,
                Id_evento,
                tipoVotacion,
                NumProy,
                permiteComentarios,
                comentariosObligatorios
            );

            votacion.Id = id;
            return votacion;
        }
    }
}

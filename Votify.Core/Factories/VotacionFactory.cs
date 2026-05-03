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
        public abstract Votacion CrearVotacion(DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy, bool permiteComentarios, bool comentariosObligatorios, EstadoVotacion estadoVotacion);
        public virtual Votacion RecrearVotacion(Guid id,DateTime fechaInicio,DateTime fechaFin,bool permiteAutoVoto,Guid Id_evento,TipoVotacion tipoVotacion,int NumProy, bool permiteComentarios, bool comentariosObligatorios, EstadoVotacion estadoVotacion)
        {
            var votacion = CrearVotacion(
                fechaInicio,
                fechaFin,
                permiteAutoVoto,
                Id_evento,
                tipoVotacion,
                NumProy,
                permiteComentarios,
                comentariosObligatorios,
                estadoVotacion
            );

            votacion.Id = id;
            return votacion;
        }
    }
}

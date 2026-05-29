using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Strategies
{
    public class VotacionJuradoStrategy : IVotacionStrategy
    {
        public void ValidarPermisoVoto(Usuario usuario, List<EventoUsuarioRol> rolesEvento)
        {
            bool esJuradoAsignado = rolesEvento.Any(e => e.Rol.Nombre == "Jurado");

            if (!esJuradoAsignado)
                throw new Exception("Solo el jurado asignado puede votar.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Strategies
{
    public class VotacionMixtaStrategy : IVotacionStrategy
    {
        public void ValidarPermisoVoto(Usuario usuario, List<EventoUsuarioRol> rolesEvento)
        {
            bool esPublico = usuario.Rol.Nombre == "Publico" ||
                rolesEvento.Any(e => e.Rol.Nombre == "Publico" ||
                                      e.Rol.Nombre == "Participante");

            bool esJuradoAsignado = rolesEvento.Any(e => e.Rol.Nombre == "Jurado");

            if (!esPublico && !esJuradoAsignado)
                throw new Exception("Solo usuarios públicos, participantes o jurado asignado pueden votar en este evento.");
        }
    }
}
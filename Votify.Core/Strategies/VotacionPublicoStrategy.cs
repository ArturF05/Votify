using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Strategies
{
    public class VotacionPublicoStrategy : IVotacionStrategy
    {
        public void ValidarPermisoVoto(Usuario usuario, List<EventoUsuarioRol> rolesEvento)
        {
            bool esPublico = usuario.Rol.Nombre == "Publico" ||
                rolesEvento.Any(e => e.Rol.Nombre == "Publico" ||
                                      e.Rol.Nombre == "Participante");

            if (!esPublico)
                throw new Exception("Solo usuarios públicos pueden votar en este evento.");
        }
    }
}
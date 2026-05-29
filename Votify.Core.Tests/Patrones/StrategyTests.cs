using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Strategies;

namespace Votify.Core.Tests.Patrones
{
    [TestClass]
    public class StrategyTests
    {
        [TestMethod]
        public void VotacionPublicoStrategy_UsuarioNoPublico_LanzaExcepcion()
        {
            var usuario = new Usuario
            {
                Rol = new Rol { Nombre = "Jurado" }
            };

            var rolesEvento = new List<EventoUsuarioRol>();

            var strategy = new VotacionPublicoStrategy();

            Assert.ThrowsException<Exception>(() =>
                strategy.ValidarPermisoVoto(usuario, rolesEvento));
        }

        [TestMethod]
        public void VotacionJuradoStrategy_UsuarioNoJurado_LanzaExcepcion()
        {
            var usuario = new Usuario
            {
                Rol = new Rol { Nombre = "Publico" }
            };

            var rolesEvento = new List<EventoUsuarioRol>();

            var strategy = new VotacionJuradoStrategy();

            Assert.ThrowsException<Exception>(() =>
                strategy.ValidarPermisoVoto(usuario, rolesEvento));
        }

        [TestMethod]
        public void VotacionMixtaStrategy_UsuarioSinRolValido_LanzaExcepcion()
        {
            var usuario = new Usuario
            {
                Rol = new Rol { Nombre = "Organizador" }
            };

            var rolesEvento = new List<EventoUsuarioRol>();

            var strategy = new VotacionMixtaStrategy();

            Assert.ThrowsException<Exception>(() =>
                strategy.ValidarPermisoVoto(usuario, rolesEvento));
        }
    }
}

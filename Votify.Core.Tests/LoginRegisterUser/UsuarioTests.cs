using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;

namespace Votify.Core.Tests.LoginRegisterUser
{
    [TestClass]
    public class UsuarioTests
    {
        [TestMethod]
        public void CrearUsuerValidSaved()
        {
            var user = new Usuario
            {
                Nombre = "nombreTest",
                Password = "passTest"
            };

            Assert.AreEqual("nombreTest", user.Nombre);
            Assert.AreEqual("passTest", user.Password);
        }

        [TestMethod]
        public void CrearUsuerNombreNotNull()
        {
            var user = new Usuario
            {
                Nombre = "nombreTest",
                Password = "passTest"
            };

            Assert.IsNotNull(user.Nombre);
        }

        [TestMethod]
        public void CrearUsuerPassNotNull()
        {
            var user = new Usuario
            {
                Nombre = "nombreTest",
                Password = "passTest"
            };

            Assert.IsNotNull(user.Password);
        }
    }
}

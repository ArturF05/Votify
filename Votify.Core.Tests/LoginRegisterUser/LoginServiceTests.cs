using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.LoginRegisterUser
{
    [TestClass]
    public class LoginServiceTests
    {
        private LoginService loginService;
        private AppDbContext context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            context = new AppDbContext(options);

            var usuarioRepository = new UsuarioRepository(context);

            loginService = new LoginService(usuarioRepository)
;
            context.Usuario.Add(new Usuario
            {
                Nombre = "usuarioTest",
                Password = "passTest"
            });

            context.SaveChanges();
        }

        [TestMethod]
        public async Task LoginValid()
        {
            bool resultado = await loginService.ValidarUsuarioAsync(
                "usuarioTest",
                "passTest"
            );

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task LoginIncorrectPass()
        {
            bool resultado = await loginService.ValidarUsuarioAsync(
                "usuarioTest",
                "IncorrectPassTest"
            );

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task LoginUserNotExists()
        {
            bool resultado = await loginService.ValidarUsuarioAsync(
                "InventadoUsuarioTest",
                "passTest"
            );

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task LoginUserNull()
        {
            bool resultado = await loginService.ValidarUsuarioAsync(
                "",
                "passTest"
            );

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task LoginPassNull()
        {
            bool resultado = await loginService.ValidarUsuarioAsync(
                "usuarioTest",
                ""
            );

            Assert.IsFalse(resultado);
        }
    }
}

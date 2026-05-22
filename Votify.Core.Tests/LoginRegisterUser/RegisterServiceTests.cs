using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.LoginRegisterUser
{
    [TestClass]
    public class RegisterServiceTests
    {
        private RegisterService registerService;
        private AppDbContext context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            context = new AppDbContext(options);

            var usuarioRepository = new UsuarioRepository(context);

            registerService = new RegisterService(usuarioRepository);
        }

        [TestMethod]
        public async Task UsuarioPuedeRegistrarse()
        {
            bool resultado = await registerService.RegistrarUsuarioAsync(
                "usuarioTest",
                "passTest",
                null
            );

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task UsuarioDuplicadoNoRegistra()
        {
            await registerService.RegistrarUsuarioAsync("usuarioDobleTest", "passTest", null);

            bool resultado = await registerService.RegistrarUsuarioAsync(
                "usuarioDobleTest",
                "passTest",
                null
            );

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task UsuarioNullNoRegistra()
        {
            bool resultado = await registerService.RegistrarUsuarioAsync(
                "",
                "passTest",
                null
            );

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task ContraseñaNullNoRegistra()
        {
            bool resultado = await registerService.RegistrarUsuarioAsync(
                "usuarioTest",
                "",
                null
            );

            Assert.IsFalse(resultado);
        }
    }
}

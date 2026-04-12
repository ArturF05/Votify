using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.Core.Services;

namespace Votify.Core.Tests.LoginRegisterUser
{
    [TestClass]
    public class LoginServiceTests
    {
        private LoginService loginService;

        [TestMethod]
        public async Task LoginValid()
        {
            var jsonRespuesta = """
            {
                "id": "11111111-1111-1111-1111-111111111111",
                "nombre": "usuarioTest",
                "rol": "Publico"
            }
            """;

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            var usuarioSesion = new UsuarioSesion();
            loginService = new LoginService(httpClient, usuarioSesion);

            bool resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "passTest");

            Assert.IsTrue(resultado);
            Assert.AreEqual(Guid.Parse("11111111-1111-1111-1111-111111111111"), usuarioSesion.IdUsuario);
            Assert.AreEqual("usuarioTest", usuarioSesion.NombreUsuario);
            Assert.AreEqual("Publico", usuarioSesion.Rol);
        }

        [TestMethod]
        public async Task LoginIncorrectPass()
        {
            var jsonRespuesta = """
            {
                "message": "Credenciales inválidas"
            }
            """;

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.Unauthorized);
            var usuarioSesion = new UsuarioSesion();
            loginService = new LoginService(httpClient, usuarioSesion);

            bool resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "IncorrectPassTest");

            Assert.IsFalse(resultado);
            Assert.AreEqual(Guid.Empty, usuarioSesion.IdUsuario);
        }

        [TestMethod]
        public async Task LoginUserNotExists()
        {
            var jsonRespuesta = """
            {
                "message": "Credenciales inválidas"
            }
            """;

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.Unauthorized);
            var usuarioSesion = new UsuarioSesion();
            loginService = new LoginService(httpClient, usuarioSesion);

            bool resultado = await loginService.ValidarUsuarioAsync("InventadoUsuarioTest", "passTest");

            Assert.IsFalse(resultado);
            Assert.AreEqual(Guid.Empty, usuarioSesion.IdUsuario);
        }

        [TestMethod]
        public async Task LoginUserNull()
        {
            var jsonRespuesta = """
            {
                "message": "Credenciales inválidas"
            }
            """;

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.Unauthorized);
            var usuarioSesion = new UsuarioSesion();
            loginService = new LoginService(httpClient, usuarioSesion);

            bool resultado = await loginService.ValidarUsuarioAsync("", "passTest");

            Assert.IsFalse(resultado);
            Assert.AreEqual(Guid.Empty, usuarioSesion.IdUsuario);
        }

        [TestMethod]
        public async Task LoginPassNull()
        {
            var jsonRespuesta = """
            {
                "message": "Credenciales inválidas"
            }
            """;

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.Unauthorized);
            var usuarioSesion = new UsuarioSesion();
            loginService = new LoginService(httpClient, usuarioSesion);

            bool resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "");

            Assert.IsFalse(resultado);
            Assert.AreEqual(Guid.Empty, usuarioSesion.IdUsuario);
        }

        private HttpClient CrearHttpClientFalso(string contenidoRespuesta, HttpStatusCode codigoEstado)
        {
            var handler = new FakeHttpMessageHandler(contenidoRespuesta, codigoEstado);

            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:5001/")
            };
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string contenidoRespuesta;
        private readonly HttpStatusCode codigoEstado;

        public FakeHttpMessageHandler(string contenidoRespuesta, HttpStatusCode codigoEstado)
        {
            this.contenidoRespuesta = contenidoRespuesta;
            this.codigoEstado = codigoEstado;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(codigoEstado)
            {
                Content = new StringContent(contenidoRespuesta, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}
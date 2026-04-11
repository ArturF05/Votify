using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Moq;
using Votify.Core.Entities;

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
                "password": "passTest",
                "id_rol": "22222222-2222-2222-2222-222222222222"
            }
            """;

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            var sessionMock = new Mock<IUsuarioSesion>();
            loginService = new LoginService(httpClient, sessionMock.Object);

            Usuario? resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "passTest");
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public async Task LoginIncorrectPass()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            var sessionMock = new Mock<IUsuarioSesion>();
            loginService = new LoginService(httpClient, sessionMock.Object);

            Usuario? resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "IncorrectPassTest");

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task LoginUserNotExists()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            var sessionMock = new Mock<IUsuarioSesion>();
            loginService = new LoginService(httpClient, sessionMock.Object);

            Usuario? resultado = await loginService.ValidarUsuarioAsync("InventadoUsuarioTest", "passTest");

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task LoginUserNull()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            var sessionMock = new Mock<IUsuarioSesion>();
            loginService = new LoginService(httpClient, sessionMock.Object);

            Usuario? resultado = await loginService.ValidarUsuarioAsync("", "passTest");

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task LoginPassNull()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            var sessionMock = new Mock<IUsuarioSesion>();
            loginService = new LoginService(httpClient, sessionMock.Object);

            Usuario? resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "");

            Assert.IsNull(resultado);
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
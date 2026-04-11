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
   /* [TestClass]
    public class LoginServiceTests
    {
        private LoginService loginService;

        [TestMethod]
        public async Task LoginValid()
        {
            var jsonRespuesta = "true";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            loginService = new LoginService(httpClient);

            bool resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "passTest");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task LoginIncorrectPass()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            loginService = new LoginService(httpClient);

            bool resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "IncorrectPassTest");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task LoginUserNotExists()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            loginService = new LoginService(httpClient);

            bool resultado = await loginService.ValidarUsuarioAsync("InventadoUsuarioTest", "passTest");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task LoginUserNull()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            loginService = new LoginService(httpClient);

            bool resultado = await loginService.ValidarUsuarioAsync("", "passTest");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task LoginPassNull()
        {
            var jsonRespuesta = "false";

            var httpClient = CrearHttpClientFalso(jsonRespuesta, HttpStatusCode.OK);
            loginService = new LoginService(httpClient);

            bool resultado = await loginService.ValidarUsuarioAsync("usuarioTest", "");

            Assert.IsFalse(resultado);
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
    }*/
}
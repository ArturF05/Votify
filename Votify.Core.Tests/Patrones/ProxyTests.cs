using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Votify.Core.Interfaces;
using Votify.Core.Services;

namespace Votify.Core.Tests.Patrones
{
    [TestClass]
    public class ProxyTests
    {
        [TestMethod]
        public async Task SubirArchivoValido_ProxyNoBloquea()
        {
            var mockRealService = new Mock<IAzureStorage>();

            mockRealService
                .Setup(x => x.SubirArchivoAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()))
                .ReturnsAsync("https://azure/test.jpg");

            var proxy = new AzureStorageProxy(mockRealService.Object);

            var contenido = new MemoryStream(Encoding.UTF8.GetBytes("contenido test"));

            var resultado = await proxy.SubirArchivoAsync(
                contenido,
                "imagen.jpg");

            Assert.AreEqual("https://azure/test.jpg", resultado);

            mockRealService.Verify(
                x => x.SubirArchivoAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public async Task SubirArchivoVacio_ProxyBloquea()
        {
            var mockRealService = new Mock<IAzureStorage>();

            var proxy = new AzureStorageProxy(mockRealService.Object);

            var contenido = new MemoryStream();

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await proxy.SubirArchivoAsync(contenido, "archivo.jpg");
            });

            mockRealService.Verify(
                x => x.SubirArchivoAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task SubirArchivo_ExtensionNoPermitida_ProxyBloquea()
        {
            var mockRealService = new Mock<IAzureStorage>();

            var proxy = new AzureStorageProxy(mockRealService.Object);

            var contenido = new MemoryStream(Encoding.UTF8.GetBytes("contenido"));

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await proxy.SubirArchivoAsync(contenido, "virus.exe");
            });

            mockRealService.Verify(
                x => x.SubirArchivoAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()),
                Times.Never);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Infrastructure.Data;

namespace Votify.Core.Tests.Proyectos
{
    [TestClass]
    public class MaterialServiceTests
    {
        private AppDbContext context;
        private IAzureStorage azure;

        private Guid proyectoId;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            context = new AppDbContext(options);
            azure = new FakeAzureStorage();

            proyectoId = Guid.NewGuid();

            context.Proyecto.Add(new Proyecto
            {
                Id = proyectoId,
                Titulo = "Proyecto Test",
                Descripcion = "Desc",
                Id_evento = Guid.NewGuid()
            });

            context.SaveChanges();
        }
        [TestMethod]
        public async Task SubirImagenAProyecto()
        {
            byte[] bytes = new byte[] { 1, 2, 3 };
            using var stream = new MemoryStream(bytes);

            // simula Azure
            var url = await azure.SubirArchivoAsync(stream, "imagen.png");

            var material = new Material
            {
                Nombre = "Imagen test",
                Tipo = "Imagen",
                Url = url,
                ProyectoId = proyectoId,
                FechaSubida = DateTime.UtcNow
            };

            context.Material.Add(material);
            await context.SaveChangesAsync();

            var materiales = await context.Material.Where(m => m.ProyectoId == proyectoId).ToListAsync();
            Assert.AreEqual(1, materiales.Count);
            Assert.AreEqual("Imagen", materiales[0].Tipo);
            Assert.IsTrue(materiales[0].Url.Contains("imagen.png"));
        }
    }
}

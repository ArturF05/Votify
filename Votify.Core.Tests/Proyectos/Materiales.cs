using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Materiales
{
    [TestClass]
    public class MaterialesTests
    {
        private AppDbContext context;
        private IMaterialRepository repository;
        private IProyectoRepository proyectoRepository;
        private IMaterialService service;
        private FakeAzureStorage azure;

        private Guid usuarioId;
        private Guid eventoId;
        private Guid proyectoId;


        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);
            repository = new MaterialRepository(context);
            proyectoRepository = new ProyectoRepository(context);
            azure = new FakeAzureStorage();
            service = new MaterialService(repository, azure, proyectoRepository);
            usuarioId = Guid.NewGuid();
            eventoId = Guid.NewGuid();
            proyectoId = Guid.NewGuid();

            context.Proyecto.Add(new Proyecto
            {
                Id = proyectoId,
                Titulo = "Proyecto Test",
                Id_evento = eventoId,
                Descripcion = "dac",
                Id_creador = usuarioId

            });

            context.SaveChanges();
        }
        [TestMethod]
        public async Task SubirImagen_Proyecto_SeGuardaCorrectamente()
        {
            var materiales = new List<MaterialDto>
            {
                new MaterialDto
                {
                    Nombre = "Imagen Test",
                    Tipo = "Imagen",
                    ArchivoBytes = new byte[] {1,2,3},
                    NombreArchivo = "imagen.jpg"
                }
            };

            await service.GuardarMateriales(proyectoId, materiales);

            var materialesDb = await context.Material.ToListAsync();

            Assert.AreEqual(1, materialesDb.Count);
            Assert.AreEqual("Imagen", materialesDb[0].Tipo);
            Assert.IsFalse(string.IsNullOrEmpty(materialesDb[0].Url));
        }

        [TestMethod]
        public async Task SubirDocumento_Proyecto_SeGuardaCorrectamente()
        {
            var materiales = new List<MaterialDto>
            {
                new MaterialDto
                {
                    Nombre = "Doc Test",
                    Tipo = "Archivo",
                    ArchivoBytes = new byte[] {5,6,7},
                    NombreArchivo = "doc.pdf",

                }
            };

            await service.GuardarMateriales(proyectoId, materiales);

            var material = await context.Material.FirstAsync();

            Assert.AreEqual("Archivo", material.Tipo);
            Assert.IsTrue(material.Url.Contains("fake-url"));
        }

        [TestMethod]
        public async Task AñadirEnlace_SeGuardaCorrectamente()
        {
            var materiales = new List<MaterialDto>
            {
                new MaterialDto
                {
                    Nombre = "Google",
                    Tipo = "Enlace",
                    Valor = "https://google.com"
                }
            };

            await service.GuardarMateriales(proyectoId, materiales);

            var material = await context.Material.FirstAsync();

            Assert.AreEqual("Enlace", material.Tipo);
            Assert.AreEqual("https://google.com", material.Url);
        }

        [TestMethod]
        public async Task AñadirVideo_SeGuardaCorrectamente()
        {
            var materiales = new List<MaterialDto>
            {
                new MaterialDto
                {
                    Nombre = "Video Test",
                    Tipo = "Video",
                    Valor = "https://youtube.com/test"
                }
            };

            await service.GuardarMateriales(proyectoId, materiales);

            var material = await context.Material.FirstAsync();

            Assert.AreEqual("Video", material.Tipo);
            Assert.AreEqual("https://youtube.com/test", material.Url);
        }

        [TestMethod]
        public async Task SubirArchivoNoPermitido_NoSeGuarda()
        {
            var materiales = new List<MaterialDto>
            {
                new MaterialDto
                {
                    Nombre = "Exe malo",
                    Tipo = "Archivo",
                    ArchivoBytes = new byte[] {9,9,9},
                    NombreArchivo = "virus.exe"

                }
            };

            await Assert.ThrowsExceptionAsync<Exception>(() =>service.GuardarMateriales(proyectoId, materiales));
        }

        [TestMethod]
        public async Task Material_ApareceEnListado()
        {
            await context.Material.AddAsync(new Material
            {
                Nombre = "Material existente",
                Tipo = "Imagen",
                Url = "url",
                ProyectoId = proyectoId
            });

            await context.SaveChangesAsync();

            var proyectos = await service.ObtenerProyectosUsuario(usuarioId);

            Assert.IsTrue(proyectos.First().Materiales.Any());
        }
    }

    //Con este código se simula Azure
    public class FakeAzureStorage : IAzureStorage
    {
        public Task<string> SubirArchivoAsync(Stream contenidoArchivo, string nombreArchivo)
        {
            return Task.FromResult("https://fake-url/" + nombreArchivo);
        }

        public Task EliminarArchivoAsync(string url)
        {
            return Task.CompletedTask;
        }
    }
}
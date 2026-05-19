using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Premios
{
    [TestClass]
    public class PremioServiceTests
    {
        private AppDbContext context = null!;
        private PremioService premioService = null!;

        private Guid idEvento;
        private Guid idOtroEvento;
        private Guid idCategoriaBackend;
        private Guid idCategoriaFrontend;
        private Guid idCategoriaOtroEvento;
        private Guid idVotacionBackend;
        private Guid idVotacionFrontend;
        private Guid idVotacionGeneral;
        private Guid idVotacionOtroEvento;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);
            premioService = new PremioService(new PremioRepository(context));

            var ahora = DateTime.UtcNow;
            var idCategoriaBase = Guid.NewGuid();

            idEvento = Guid.NewGuid();
            idOtroEvento = Guid.NewGuid();
            idCategoriaBackend = Guid.NewGuid();
            idCategoriaFrontend = Guid.NewGuid();
            idCategoriaOtroEvento = Guid.NewGuid();
            idVotacionBackend = Guid.NewGuid();
            idVotacionFrontend = Guid.NewGuid();
            idVotacionGeneral = Guid.NewGuid();
            idVotacionOtroEvento = Guid.NewGuid();

            var categoriaBase = new Categoria
            {
                Id = idCategoriaBase,
                Nombre = "Tecnologia"
            };

            context.Categoria.Add(categoriaBase);

            context.Evento.AddRange(
                new EventoHackathon
                {
                    Id = idEvento,
                    Nombre = "Hackathon principal",
                    Descripcion = "Evento de prueba",
                    FechaInicio = ahora.AddDays(-1),
                    FechaFin = ahora.AddDays(5),
                    Id_categoria = idCategoriaBase,
                    Categoria = categoriaBase
                },
                new EventoHackathon
                {
                    Id = idOtroEvento,
                    Nombre = "Otro hackathon",
                    Descripcion = "Otro evento",
                    FechaInicio = ahora.AddDays(-1),
                    FechaFin = ahora.AddDays(5),
                    Id_categoria = idCategoriaBase,
                    Categoria = categoriaBase
                }
            );

            context.CategoriaEvento.AddRange(
                new CategoriaEvento
                {
                    Id = idCategoriaBackend,
                    Id_evento = idEvento,
                    Nombre = "Backend"
                },
                new CategoriaEvento
                {
                    Id = idCategoriaFrontend,
                    Id_evento = idEvento,
                    Nombre = "Frontend"
                },
                new CategoriaEvento
                {
                    Id = idCategoriaOtroEvento,
                    Id_evento = idOtroEvento,
                    Nombre = "Otra categoria"
                }
            );

            context.Votacion.AddRange(
                new VotacionPublico
                {
                    Id = idVotacionBackend,
                    Id_evento = idEvento,
                    Id_categoria_evento = idCategoriaBackend,
                    FechaInicio = ahora.AddHours(-1),
                    FechaFin = ahora.AddHours(4),
                    NumProy = 1,
                    EstadoVotacion = EstadoVotacion.Abierta
                },
                new VotacionPublico
                {
                    Id = idVotacionFrontend,
                    Id_evento = idEvento,
                    Id_categoria_evento = idCategoriaFrontend,
                    FechaInicio = ahora.AddHours(-1),
                    FechaFin = ahora.AddHours(4),
                    NumProy = 1,
                    EstadoVotacion = EstadoVotacion.Abierta
                },
                new VotacionPublico
                {
                    Id = idVotacionGeneral,
                    Id_evento = idEvento,
                    Id_categoria_evento = null,
                    FechaInicio = ahora.AddHours(-1),
                    FechaFin = ahora.AddHours(4),
                    NumProy = 1,
                    EstadoVotacion = EstadoVotacion.Abierta
                },
                new VotacionPublico
                {
                    Id = idVotacionOtroEvento,
                    Id_evento = idOtroEvento,
                    Id_categoria_evento = idCategoriaOtroEvento,
                    FechaInicio = ahora.AddHours(-1),
                    FechaFin = ahora.AddHours(4),
                    NumProy = 1,
                    EstadoVotacion = EstadoVotacion.Abierta
                }
            );

            context.DimensionesDeValor.AddRange(
                new DimensionesDeValor
                {
                    Id = Guid.NewGuid(),
                    Id_votacion = idVotacionBackend,
                    Id_categoria_evento = idCategoriaBackend,
                    Nombre = "Calidad",
                    Descripcion = "Calidad tecnica",
                    Peso = 100
                },
                new DimensionesDeValor
                {
                    Id = Guid.NewGuid(),
                    Id_votacion = idVotacionFrontend,
                    Id_categoria_evento = idCategoriaFrontend,
                    Nombre = "Diseno",
                    Descripcion = "Diseno visual",
                    Peso = 100
                },
                new DimensionesDeValor
                {
                    Id = Guid.NewGuid(),
                    Id_votacion = idVotacionGeneral,
                    Id_categoria_evento = idCategoriaBackend,
                    Nombre = "Backend general",
                    Descripcion = "Criterio backend",
                    Peso = 50
                },
                new DimensionesDeValor
                {
                    Id = Guid.NewGuid(),
                    Id_votacion = idVotacionGeneral,
                    Id_categoria_evento = idCategoriaFrontend,
                    Nombre = "Frontend general",
                    Descripcion = "Criterio frontend",
                    Peso = 50
                }
            );

            context.SaveChanges();
        }

        [TestMethod]
        public async Task CrearPremio_CategoriaValida_GuardaPremio()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                " Mejor API ",
                " Entrega mas solida ",
                1,
                new[] { idVotacionBackend },
                imagenUrl: " https://storage.test/premio.png ",
                puesto: 1);

            var premioGuardado = await context.Premio.SingleAsync();

            Assert.AreEqual(premio.Id, premioGuardado.Id);
            Assert.AreEqual(idCategoriaBackend, premioGuardado.Id_categoria_evento);
            Assert.AreEqual("Mejor API", premioGuardado.Nombre);
            Assert.AreEqual("Entrega mas solida", premioGuardado.Descripcion);
            Assert.AreEqual("https://storage.test/premio.png", premioGuardado.ImagenUrl);
            Assert.AreEqual(1, premioGuardado.Orden);
            Assert.AreEqual(1, premioGuardado.Puesto);
            Assert.IsTrue(await context.PremioVotacion.AnyAsync(pv =>
                pv.Id_premio == premioGuardado.Id &&
                pv.Id_votacion == idVotacionBackend));
        }

        [TestMethod]
        public async Task EditarPremio_ActualizaImagenPuestoYVotaciones()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionBackend },
                imagenUrl: "https://storage.test/old.png");

            var premioEditado = await premioService.EditarPremioAsync(
                premio.Id,
                "Mejor backend",
                "Nueva descripcion",
                2,
                "https://storage.test/new.png",
                puesto: 1,
                idsVotaciones: new[] { idVotacionGeneral });

            Assert.AreEqual("Mejor backend", premioEditado.Nombre);
            Assert.AreEqual("Nueva descripcion", premioEditado.Descripcion);
            Assert.AreEqual("https://storage.test/new.png", premioEditado.ImagenUrl);
            Assert.AreEqual(2, premioEditado.Orden);
            Assert.AreEqual(1, premioEditado.Puesto);
            Assert.IsTrue(await context.PremioVotacion.AnyAsync(pv =>
                pv.Id_premio == premio.Id &&
                pv.Id_votacion == idVotacionGeneral));
            Assert.IsFalse(await context.PremioVotacion.AnyAsync(pv =>
                pv.Id_premio == premio.Id &&
                pv.Id_votacion == idVotacionBackend));
        }

        [TestMethod]
        public async Task CrearPremio_NombreDuplicadoEnCategoria_LanzaExcepcion()
        {
            await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionBackend });

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => premioService.CrearPremioAsync(
                    idCategoriaBackend,
                    "mejor api",
                    null,
                    2,
                    new[] { idVotacionBackend }));

            Assert.AreEqual("Ya existe un premio con ese nombre en la categoria.", exception.Message);
        }

        [TestMethod]
        public async Task CrearPremio_SinVotacion_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => premioService.CrearPremioAsync(idCategoriaBackend, "Mejor API", null, 1));

            Assert.AreEqual("El premio debe estar vinculado al menos a una votacion.", exception.Message);
        }

        [TestMethod]
        public async Task CrearPremio_PuestoFueraDeLaVotacion_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => premioService.CrearPremioAsync(
                    idCategoriaBackend,
                    "Mejor API",
                    null,
                    1,
                    new[] { idVotacionBackend },
                    puesto: 2));

            Assert.AreEqual("El puesto 2 no existe en una de las votaciones seleccionadas.", exception.Message);
        }

        [TestMethod]
        public async Task ActualizarVotacionesPremio_VotacionDeMismaCategoria_CreaVinculo()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionGeneral });

            var vinculos = await premioService.ActualizarVotacionesPremioAsync(
                premio.Id,
                new[] { idVotacionBackend });

            Assert.AreEqual(1, vinculos.Count);
            Assert.AreEqual(idVotacionBackend, vinculos[0].Id_votacion);
            Assert.IsTrue(await context.PremioVotacion.AnyAsync(pv =>
                pv.Id_premio == premio.Id &&
                pv.Id_votacion == idVotacionBackend));
        }

        [TestMethod]
        public async Task ActualizarVotacionesPremio_VotacionDeOtraCategoria_LanzaExcepcion()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionBackend });

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => premioService.ActualizarVotacionesPremioAsync(
                    premio.Id,
                    new[] { idVotacionFrontend }));

            Assert.AreEqual("La votacion no corresponde a la categoria del premio.", exception.Message);
        }

        [TestMethod]
        public async Task ActualizarVotacionesPremio_VotacionGeneralConCriterios_CreaVinculo()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionBackend });

            var vinculos = await premioService.ActualizarVotacionesPremioAsync(
                premio.Id,
                new[] { idVotacionGeneral });

            Assert.AreEqual(1, vinculos.Count);
            Assert.AreEqual(idVotacionGeneral, vinculos[0].Id_votacion);
        }

        [TestMethod]
        public async Task ActualizarVotacionesPremio_SincronizaAltasYBajas()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionBackend, idVotacionGeneral });

            var vinculos = await premioService.ActualizarVotacionesPremioAsync(
                premio.Id,
                new[] { idVotacionGeneral });

            Assert.AreEqual(1, vinculos.Count);
            Assert.AreEqual(idVotacionGeneral, vinculos[0].Id_votacion);
            Assert.IsFalse(await context.PremioVotacion.AnyAsync(pv =>
                pv.Id_premio == premio.Id &&
                pv.Id_votacion == idVotacionBackend));
        }

        [TestMethod]
        public async Task ActualizarVotacionesPremio_SinVotaciones_LanzaExcepcion()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionBackend });

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => premioService.ActualizarVotacionesPremioAsync(premio.Id, Array.Empty<Guid>()));

            Assert.AreEqual("El premio debe estar vinculado al menos a una votacion.", exception.Message);
        }

        [TestMethod]
        public async Task ObtenerPremiosPorVotacion_FiltraPorCategoria()
        {
            await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionGeneral });

            await premioService.CrearPremioAsync(
                idCategoriaFrontend,
                "Mejor interfaz",
                null,
                1,
                new[] { idVotacionGeneral });

            var premiosBackend = await premioService.ObtenerPremiosPorVotacionAsync(
                idVotacionGeneral,
                idCategoriaBackend);

            Assert.AreEqual(1, premiosBackend.Count);
            Assert.AreEqual("Mejor API", premiosBackend[0].Nombre);
            Assert.AreEqual(idCategoriaBackend, premiosBackend[0].Id_categoria_evento);
        }

        [TestMethod]
        public async Task ActualizarVotacionesPremio_VotacionDeOtroEvento_LanzaExcepcion()
        {
            var premio = await premioService.CrearPremioAsync(
                idCategoriaBackend,
                "Mejor API",
                null,
                1,
                new[] { idVotacionBackend });

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => premioService.ActualizarVotacionesPremioAsync(
                    premio.Id,
                    new[] { idVotacionOtroEvento }));

            Assert.AreEqual("La votacion no pertenece al evento de la categoria del premio.", exception.Message);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}

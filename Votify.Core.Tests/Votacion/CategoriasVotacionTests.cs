using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Observer;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Votacion
{
    [TestClass]
    public class CategoriasVotacionTests
    {
        private AppDbContext context = null!;
        private VotacionService votacionService = null!;

        private Guid idEvento;
        private Guid idOtroEvento;
        private Guid idUsuarioPublico;

        private Guid idCategoriaEventoBackend;
        private Guid idCategoriaEventoFrontend;
        private Guid idCategoriaEventoOtroEvento;

        private Guid idVotacionGeneral;
        private Guid idVotacionBackend;

        private Guid idProyectoBackend;
        private Guid idProyectoFrontend;

        private Guid idCriterioBackendCalidad;
        private Guid idCriterioBackendPresentacion;
        private Guid idCriterioGeneralBackend;
        private Guid idCriterioGeneralFrontend;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);

            votacionService = new VotacionService(
                new VotacionRepository(context),
                new VotoRepository(context),
                new ProyectoRepository(context),
                new DimensionRepository(context),
                new UsuarioRepository(context),
                new EventoUsuarioRolRepository(context),
                new Mock<IEmailSender>().Object
            );

            var ahora = DateTime.UtcNow;
            var idCategoriaBase = Guid.NewGuid();

            idEvento = Guid.NewGuid();
            idOtroEvento = Guid.NewGuid();
            idUsuarioPublico = Guid.NewGuid();

            idCategoriaEventoBackend = Guid.NewGuid();
            idCategoriaEventoFrontend = Guid.NewGuid();
            idCategoriaEventoOtroEvento = Guid.NewGuid();

            idVotacionGeneral = Guid.NewGuid();
            idVotacionBackend = Guid.NewGuid();

            idProyectoBackend = Guid.NewGuid();
            idProyectoFrontend = Guid.NewGuid();

            idCriterioBackendCalidad = Guid.NewGuid();
            idCriterioBackendPresentacion = Guid.NewGuid();
            idCriterioGeneralBackend = Guid.NewGuid();
            idCriterioGeneralFrontend = Guid.NewGuid();

            var categoriaBase = new Categoria
            {
                Id = idCategoriaBase,
                Nombre = "Tecnología"
            };

            var rolPublico = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = "Publico"
            };

            context.Categoria.Add(categoriaBase);
            context.Role.Add(rolPublico);

            context.Usuario.Add(new Usuario
            {
                Id = idUsuarioPublico,
                Nombre = "publico",
                Password = "1234",
                Id_rol = rolPublico.Id,
                Rol = rolPublico
            });

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
                    Id = idCategoriaEventoBackend,
                    Id_evento = idEvento,
                    Nombre = "Backend"
                },
                new CategoriaEvento
                {
                    Id = idCategoriaEventoFrontend,
                    Id_evento = idEvento,
                    Nombre = "Frontend"
                },
                new CategoriaEvento
                {
                    Id = idCategoriaEventoOtroEvento,
                    Id_evento = idOtroEvento,
                    Nombre = "Categoría de otro evento"
                }
            );

            context.EventoUsuarioRol.Add(new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idUsuarioPublico,
                RolId = rolPublico.Id,
                Rol = rolPublico
            });

            context.Proyecto.AddRange(
                new Proyecto
                {
                    Id = idProyectoBackend,
                    Titulo = "API",
                    Descripcion = "Proyecto backend",
                    Id_evento = idEvento,
                    Id_categoria_evento = idCategoriaEventoBackend,
                    ProyectoParticipantes = new List<ProyectoParticipantes>(),
                    Materiales = new List<Material>()
                },
                new Proyecto
                {
                    Id = idProyectoFrontend,
                    Titulo = "Web",
                    Descripcion = "Proyecto frontend",
                    Id_evento = idEvento,
                    Id_categoria_evento = idCategoriaEventoFrontend,
                    ProyectoParticipantes = new List<ProyectoParticipantes>(),
                    Materiales = new List<Material>()
                }
            );

            context.Votacion.AddRange(
                new VotacionPublico
                {
                    Id = idVotacionGeneral,
                    Id_evento = idEvento,
                    Id_categoria_evento = null,
                    FechaInicio = ahora.AddHours(-1),
                    FechaFin = ahora.AddHours(4),
                    PermiteAutoVoto = true,
                    NumProy = 1,
                    EstadoVotacion = EstadoVotacion.Abierta
                },
                new VotacionPublico
                {
                    Id = idVotacionBackend,
                    Id_evento = idEvento,
                    Id_categoria_evento = idCategoriaEventoBackend,
                    FechaInicio = ahora.AddHours(-1),
                    FechaFin = ahora.AddHours(4),
                    PermiteAutoVoto = true,
                    NumProy = 1,
                    EstadoVotacion = EstadoVotacion.Abierta
                }
            );

            context.DimensionesDeValor.AddRange(
                new DimensionesDeValor
                {
                    Id = idCriterioBackendCalidad,
                    Id_votacion = idVotacionBackend,
                    Id_categoria_evento = idCategoriaEventoBackend,
                    Nombre = "Calidad",
                    Descripcion = "Calidad técnica",
                    Peso = 60
                },
                new DimensionesDeValor
                {
                    Id = idCriterioBackendPresentacion,
                    Id_votacion = idVotacionBackend,
                    Id_categoria_evento = idCategoriaEventoBackend,
                    Nombre = "Presentación",
                    Descripcion = "Claridad",
                    Peso = 40
                },
                new DimensionesDeValor
                {
                    Id = idCriterioGeneralBackend,
                    Id_votacion = idVotacionGeneral,
                    Id_categoria_evento = idCategoriaEventoBackend,
                    Nombre = "Backend general",
                    Descripcion = "Criterio backend",
                    Peso = 100
                },
                new DimensionesDeValor
                {
                    Id = idCriterioGeneralFrontend,
                    Id_votacion = idVotacionGeneral,
                    Id_categoria_evento = idCategoriaEventoFrontend,
                    Nombre = "Frontend general",
                    Descripcion = "Criterio frontend",
                    Peso = 100
                }
            );

            context.SaveChanges();
        }

        [TestMethod]
        public async Task CrearVotacion_ConCategoriaValida_GuardaCategoriaYCriterios()
        {
            var idsAntes = await context.Votacion.Select(v => v.Id).ToListAsync();

            var criterios = new List<CrearCriterioDTO>
            {
                new CrearCriterioDTO
                {
                    IdCategoriaEvento = idCategoriaEventoBackend,
                    Nombre = "Arquitectura",
                    Descripcion = "Diseño técnico",
                    Peso = 70
                },
                new CrearCriterioDTO
                {
                    IdCategoriaEvento = idCategoriaEventoBackend,
                    Nombre = "Documentación",
                    Descripcion = "Claridad documental",
                    Peso = 30
                }
            };

            await votacionService.CrearVotacion(
                idEvento,
                idCategoriaEventoBackend,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                true,
                TipoVotacion.Publico,
                1,
                true,
                false,
                true,
                new List<Guid>(),
                EstadoVotacion.Abierta,
                criterios
            );

            var votacionCreada = await context.Votacion
                .FirstAsync(v => !idsAntes.Contains(v.Id));

            var criteriosGuardados = await context.DimensionesDeValor
                .Where(c => c.Id_votacion == votacionCreada.Id)
                .ToListAsync();

            Assert.AreEqual(idCategoriaEventoBackend, votacionCreada.Id_categoria_evento);
            Assert.AreEqual(2, criteriosGuardados.Count);
            Assert.IsTrue(criteriosGuardados.All(c => c.Id_categoria_evento == idCategoriaEventoBackend));
        }

        [TestMethod]
        public async Task CrearVotacion_CategoriaDeOtroEvento_LanzaExcepcion()
        {
            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.CrearVotacion(
                    idEvento,
                    idCategoriaEventoOtroEvento,
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(2),
                    true,
                    TipoVotacion.Publico,
                    1,
                    true,
                    false,
                    true,
                    new List<Guid>(),
                    EstadoVotacion.Abierta,
                    new List<CrearCriterioDTO>()
                )
            );

            Assert.AreEqual("La categoría de la votación no pertenece al evento", exception.Message);
        }

        [TestMethod]
        public async Task CrearVotacion_CriterioSinCategoria_LanzaExcepcion()
        {
            var criterios = new List<CrearCriterioDTO>
            {
                new CrearCriterioDTO
                {
                    IdCategoriaEvento = Guid.Empty,
                    Nombre = "Calidad",
                    Descripcion = "Calidad general",
                    Peso = 100
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.CrearVotacion(
                    idEvento,
                    null,
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(2),
                    true,
                    TipoVotacion.Publico,
                    1,
                    true,
                    false,
                    true,
                    new List<Guid>(),
                    EstadoVotacion.Abierta,
                    criterios
                )
            );

            Assert.AreEqual("Cada criterio debe pertenecer a una categoría del evento.", exception.Message);
        }

        [TestMethod]
        public async Task EditarVotacion_CambiaCategoriaYActualizaCriterios()
        {
            var criterios = new List<CrearCriterioDTO>
            {
                new CrearCriterioDTO
                {
                    Id = idCriterioGeneralFrontend,
                    IdCategoriaEvento = idCategoriaEventoFrontend,
                    Nombre = "Diseño visual",
                    Descripcion = "Calidad visual actualizada",
                    Peso = 100
                }
            };

            await votacionService.EditarVotacion(
                idVotacionGeneral,
                idEvento,
                idCategoriaEventoFrontend,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddHours(5),
                true,
                TipoVotacion.Publico,
                criterios,
                1,
                true,
                false,
                true,
                new List<Guid>(),
                EstadoVotacion.Abierta
            );

            var votacionEditada = await context.Votacion.FindAsync(idVotacionGeneral);
            var criteriosActuales = await context.DimensionesDeValor
                .Where(c => c.Id_votacion == idVotacionGeneral)
                .ToListAsync();

            Assert.IsNotNull(votacionEditada);
            Assert.AreEqual(idCategoriaEventoFrontend, votacionEditada.Id_categoria_evento);
            Assert.AreEqual(1, criteriosActuales.Count);
            Assert.AreEqual(idCategoriaEventoFrontend, criteriosActuales[0].Id_categoria_evento);
            Assert.AreEqual("Diseño visual", criteriosActuales[0].Nombre);
        }

        [TestMethod]
        public async Task RegistrarVoto_CategoriaValida_GuardaCategoriaYPuntuacion()
        {
            var dto = CrearDtoVotoBackend();

            var resultado = await votacionService.RegistrarVotoAsync(idVotacionBackend, dto);

            var voto = await context.Voto
                .Include(v => v.VotosCriterio)
                .SingleAsync(v => v.Id_votacion == idVotacionBackend);

            Assert.IsTrue(resultado);
            Assert.AreEqual(idCategoriaEventoBackend, voto.Id_categoria);
            Assert.AreEqual(8m, voto.Puntuacion);
            Assert.AreEqual(2, voto.VotosCriterio.Count);
        }

        [TestMethod]
        public async Task RegistrarVoto_SinCategoria_LanzaExcepcion()
        {
            var dto = CrearDtoVotoBackend();
            dto.Votos[0].IdCategoriaEvento = Guid.Empty;

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacionBackend, dto)
            );

            Assert.AreEqual("Debes indicar la categoría del evento que estás votando.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVoto_CategoriaNoPerteneceAlEvento_LanzaExcepcion()
        {
            var dto = CrearDtoVotoBackend();
            dto.Votos[0].IdCategoriaEvento = idCategoriaEventoOtroEvento;

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacionBackend, dto)
            );

            Assert.AreEqual("La categoría seleccionada no pertenece al evento de la votación.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVoto_ProyectoDeOtraCategoria_LanzaExcepcion()
        {
            var dto = CrearDtoVotoBackend();
            dto.Votos[0].IdProyecto = idProyectoFrontend;

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacionBackend, dto)
            );

            Assert.AreEqual("El proyecto no pertenece a la categoría seleccionada.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVoto_VotacionDeCategoriaDistinta_LanzaExcepcion()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idUsuarioPublico,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO
                    {
                        IdProyecto = idProyectoFrontend,
                        IdCategoriaEvento = idCategoriaEventoFrontend,
                        Puntuaciones = new List<PuntuacionCriterioDTO>
                        {
                            new PuntuacionCriterioDTO
                            {
                                IdCriterio = idCriterioGeneralFrontend,
                                Nota = 8
                            }
                        }
                    }
                }
            };

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacionBackend, dto)
            );

            Assert.AreEqual("Esta votación no corresponde a la categoría seleccionada.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVoto_DosVecesMismaCategoria_LanzaExcepcion()
        {
            await votacionService.RegistrarVotoAsync(idVotacionBackend, CrearDtoVotoBackend());

            var exception = await Assert.ThrowsExceptionAsync<Exception>(
                () => votacionService.RegistrarVotoAsync(idVotacionBackend, CrearDtoVotoBackend())
            );

            Assert.AreEqual("Ya has votado en esta categoría.", exception.Message);
        }

        [TestMethod]
        public async Task RegistrarVoto_VotacionGeneral_PermiteVotarCategoriasDistintas()
        {
            await votacionService.RegistrarVotoAsync(idVotacionGeneral, new EmitirVotoDTO
            {
                IdVotante = idUsuarioPublico,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO
                    {
                        IdProyecto = idProyectoBackend,
                        IdCategoriaEvento = idCategoriaEventoBackend,
                        Puntuaciones = new List<PuntuacionCriterioDTO>
                        {
                            new PuntuacionCriterioDTO
                            {
                                IdCriterio = idCriterioGeneralBackend,
                                Nota = 7
                            }
                        }
                    }
                }
            });

            await votacionService.RegistrarVotoAsync(idVotacionGeneral, new EmitirVotoDTO
            {
                IdVotante = idUsuarioPublico,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO
                    {
                        IdProyecto = idProyectoFrontend,
                        IdCategoriaEvento = idCategoriaEventoFrontend,
                        Puntuaciones = new List<PuntuacionCriterioDTO>
                        {
                            new PuntuacionCriterioDTO
                            {
                                IdCriterio = idCriterioGeneralFrontend,
                                Nota = 9
                            }
                        }
                    }
                }
            });

            var votos = await context.Voto
                .Where(v => v.Id_votacion == idVotacionGeneral && v.Id_votante == idUsuarioPublico)
                .ToListAsync();

            Assert.AreEqual(2, votos.Count);
            Assert.IsTrue(votos.Any(v => v.Id_categoria == idCategoriaEventoBackend));
            Assert.IsTrue(votos.Any(v => v.Id_categoria == idCategoriaEventoFrontend));
        }

        private EmitirVotoDTO CrearDtoVotoBackend()
        {
            return new EmitirVotoDTO
            {
                IdVotante = idUsuarioPublico,
                IdEvento = idEvento,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO
                    {
                        IdProyecto = idProyectoBackend,
                        IdCategoriaEvento = idCategoriaEventoBackend,
                        Puntuaciones = new List<PuntuacionCriterioDTO>
                        {
                            new PuntuacionCriterioDTO
                            {
                                IdCriterio = idCriterioBackendCalidad,
                                Nota = 10,
                                Comentario = "Muy sólido"
                            },
                            new PuntuacionCriterioDTO
                            {
                                IdCriterio = idCriterioBackendPresentacion,
                                Nota = 5,
                                Comentario = "Mejorable"
                            }
                        }
                    }
                }
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}

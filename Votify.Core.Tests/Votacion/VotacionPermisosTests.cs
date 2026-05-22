using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Factories;
using Votify.Core.Observer;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Votaciones
{
    [TestClass]
    public class VotacionPermisosTests
    {
        private AppDbContext context;

        private VotacionRepository votacionRepository;
        private VotoRepository votoRepository;
        private ProyectoRepository proyectoRepository;
        DimensionRepository dimensionRepository;
        private VotacionService votacionService;
        private UsuarioRepository usuarioRepository;
        private EventoUsuarioRolRepository eventoUsuarioRolRepository;

        private Guid idEvento;
        private Guid idCategoriaEvento;
        private Guid idUsuarioParticipante;
        private Guid idUsuarioPublico;
        private Guid idUsuarioAjeno1;
        private Guid idUsuarioJurado;
        private Guid idUsuarioAjeno2;

        private Guid idProyectoPropio;
        private Guid idProyectoAjeno1;
        private Guid idProyectoAjeno2;
        private List<ProyectoParticipantes> pPPropio = new List<ProyectoParticipantes>();
        private List<ProyectoParticipantes> pPAjeno1 = new List<ProyectoParticipantes>();
        private List<ProyectoParticipantes> pPAjeno2 = new List<ProyectoParticipantes>();

        private List<EventoUsuarioRol> eurParticipante = new();
        private List<EventoUsuarioRol> eurPublico = new();


        private Guid idVotacionPublicaConAutovoto;
        private Guid idVotacionPublicaSinAutovoto;
        private Guid idVotacionPublica;
        private Guid idVotacionJurado;
        private Guid idVotacionMixta;

        [TestInitialize]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);

            votacionRepository = new VotacionRepository(context);
            votoRepository = new VotoRepository(context);
            proyectoRepository = new ProyectoRepository(context);
            dimensionRepository = new DimensionRepository(context);
            usuarioRepository = new UsuarioRepository(context);
            eventoUsuarioRolRepository = new EventoUsuarioRolRepository(context);

            votacionService = new VotacionService(
                votacionRepository,
                votoRepository,
                proyectoRepository,
                dimensionRepository,
                usuarioRepository,
                eventoUsuarioRolRepository,
                new Mock<IEmailSender>().Object
            );

            idEvento = Guid.NewGuid();
            idCategoriaEvento = Guid.NewGuid();
            idUsuarioParticipante = Guid.NewGuid();
            idUsuarioPublico = Guid.NewGuid();
            idUsuarioJurado = Guid.NewGuid();
            idUsuarioAjeno1 = Guid.NewGuid();
            idUsuarioAjeno2 = Guid.NewGuid();

            idProyectoPropio = Guid.NewGuid();
            idProyectoAjeno1 = Guid.NewGuid();
            idProyectoAjeno2 = Guid.NewGuid();

            var relacionPropio = new ProyectoParticipantes
            {
                Id_proyecto = idProyectoPropio,
                Id_usuario = idUsuarioParticipante
            };

            var relacionAjeno1 = new ProyectoParticipantes
            {
                Id_proyecto = idProyectoAjeno1,
                Id_usuario = Guid.NewGuid()
            };

            var relacionAjeno2 = new ProyectoParticipantes
            {
                Id_proyecto = idProyectoAjeno2,
                Id_usuario = Guid.NewGuid()
            };

            idVotacionPublicaConAutovoto = Guid.NewGuid();
            idVotacionPublicaSinAutovoto = Guid.NewGuid();
            idVotacionPublica = Guid.NewGuid();
            idVotacionJurado = Guid.NewGuid();
            idVotacionMixta = Guid.NewGuid();
            var rolParticipante = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = "Participante"
            };

            var rolPublico = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = "Publico"
            };
            var rolJurado = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = "Jurado"
            };
            var evento = new Evento
            {
                Id = idEvento,
                Nombre = "Evento Test",
                Descripcion = "Evento para pruebas",
                FechaInicio = DateTime.Today.AddDays(-2),
                FechaFin = DateTime.Today.AddDays(2)
            };

            var categoriaEvento = new CategoriaEvento
            {
                Id = idCategoriaEvento,
                Id_evento = idEvento,
                Nombre = "Categoría test"
            };

            var usuarioParticipante = new Usuario
            {
                Id = idUsuarioParticipante,
                Nombre = "participante1",
                Password = "1234",
                Id_rol = rolParticipante.Id,
                Rol = rolParticipante
            };

            var usuarioPublico = new Usuario
            {
                Id = idUsuarioPublico,
                Nombre = "publico1",
                Password = "1234",
                Id_rol = rolPublico.Id,
                Rol = rolPublico
            };

            EventoUsuarioRol eventoUsuarioRolParticipante = new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idUsuarioParticipante,
                RolId = rolParticipante.Id,
                Rol = rolParticipante
            };
            eurParticipante.Add(eventoUsuarioRolParticipante);
            EventoUsuarioRol eventoUsuarioRolPublico = new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idUsuarioPublico,
                RolId = rolPublico.Id,
                Rol = rolPublico
            };
            eurPublico.Add(eventoUsuarioRolPublico);
            EventoUsuarioRol eventoUsuarioRolJurado = new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idUsuarioJurado,
                RolId = rolJurado.Id,
                Rol = rolJurado
            };
            context.EventoUsuarioRol.Add(eventoUsuarioRolJurado);

            var proyectoPropio = new Proyecto
            {
                Id = idProyectoPropio,
                Titulo = "Proyecto Propio",
                Descripcion = "Descripcion del proyecto propio",
                Id_evento = idEvento,
                Id_categoria_evento = idCategoriaEvento,
                Id_creador = idUsuarioParticipante,
                ProyectoParticipantes = new List<ProyectoParticipantes>
                {
                    new ProyectoParticipantes
                    {
                        Id_proyecto = idProyectoPropio,
                        Id_usuario = idUsuarioParticipante
                    }
                }
            };

            var proyectoAjeno1 = new Proyecto
            {
                Id = idProyectoAjeno1,
                Titulo = "Proyecto Ajeno 1",
                Descripcion = "Descripcion del proyecto ajeno 1",
                Id_evento = idEvento,
                Id_categoria_evento = idCategoriaEvento,
                Id_creador = idUsuarioAjeno1,
                ProyectoParticipantes = new List<ProyectoParticipantes>
                {
                    new ProyectoParticipantes
                    {
                        Id_proyecto = idProyectoAjeno1,
                        Id_usuario = idUsuarioAjeno1
                    }
                }
            };

            var proyectoAjeno2 = new Proyecto
            {
                Id = idProyectoAjeno2,
                Titulo = "Proyecto Ajeno 2",
                Descripcion = "Descripcion del proyecto ajeno 2",
                Id_evento = idEvento,
                Id_categoria_evento = idCategoriaEvento,
                Id_creador = Guid.NewGuid(),
                ProyectoParticipantes = new List<ProyectoParticipantes>
                {
                    new ProyectoParticipantes
                    {
                        Id_proyecto = idProyectoAjeno2,
                        Id_usuario = idUsuarioAjeno2
                    }
                }
            };

            var ahora = DateTime.UtcNow;

            var votacionPublicaConAutovoto = new VotacionPublico
            {
                Id = idVotacionPublicaConAutovoto,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = true,
                NumProy = 3
            };

            var votacionPublicaSinAutovoto = new VotacionPublico
            {
                Id = idVotacionPublicaSinAutovoto,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = false,
                NumProy = 3
            };

            var votacionPublica = new VotacionPublico
            {
                Id = idVotacionPublica,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = true,
                NumProy = 3
            };

            var votacionJurado = new VotacionJurado
            {
                Id = idVotacionJurado,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = false,
                NumProy = 3
            };

            var votacionMixta = new VotacionMixta
            {
                Id = idVotacionMixta,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = false,
                NumProy = 3
            };
            var criterio = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacionPublicaConAutovoto,
                Id_categoria_evento = idCategoriaEvento,
                Nombre = "Calidad",
                Descripcion = "Criterio de prueba",
                Peso = 100
            };

            var criterio2 = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacionPublicaSinAutovoto,
                Id_categoria_evento = idCategoriaEvento,
                Nombre = "Calidad",
                Descripcion = "Criterio de prueba",
                Peso = 100
            };

            var criterio3 = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacionPublica,
                Id_categoria_evento = idCategoriaEvento,
                Nombre = "Calidad",
                Descripcion = "Criterio de prueba",
                Peso = 100
            };

            context.DimensionesDeValor.AddRange(criterio, criterio2, criterio3);
            context.Role.AddRange(rolParticipante, rolPublico, rolJurado);
            context.Usuario.AddRange(usuarioParticipante, usuarioPublico);
            context.Evento.Add(evento);
            context.CategoriaEvento.Add(categoriaEvento);
            context.EventoUsuarioRol.AddRange(eventoUsuarioRolParticipante, eventoUsuarioRolPublico);
            context.Proyecto.AddRange(proyectoPropio, proyectoAjeno1, proyectoAjeno2);
            context.Votacion.AddRange(
                votacionPublicaConAutovoto,
                votacionPublicaSinAutovoto,
                votacionPublica,
                votacionJurado,
                votacionMixta
            );

            await context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task Participante_PuedeAutovotarse_SoloSiLaVotacionLoPermite()
        {
            var dtoPermite = new EmitirVotoDTO
            {
                IdVotante = idUsuarioParticipante,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyectoPropio, IdCategoriaEvento = idCategoriaEvento },
                    new VotoDetalleDTO { IdProyecto = idProyectoAjeno1, IdCategoriaEvento = idCategoriaEvento },
                    new VotoDetalleDTO { IdProyecto = idProyectoAjeno2, IdCategoriaEvento = idCategoriaEvento }
                },
                IdEvento = idEvento
            };

            Assert.IsNotNull(dtoPermite);
            Assert.IsNotNull(dtoPermite.Votos);
            Assert.AreEqual(3, dtoPermite.Votos.Count);

            var resultadoPermite = await votacionService.RegistrarVotoAsync(idVotacionPublicaConAutovoto, dtoPermite);

            Assert.IsTrue(resultadoPermite, "Debería permitir votar al propio proyecto cuando el autovoto está permitido.");

            var votosGuardadosPermite = await context.Voto
                .Where(v => v.Id_votacion == idVotacionPublicaConAutovoto && v.Id_votante == idUsuarioParticipante)
                .ToListAsync();

            Assert.AreEqual(3, votosGuardadosPermite.Count, "Deben guardarse 3 votos cuando el autovoto está permitido.");

            var dtoNoPermite = new EmitirVotoDTO
            {
                IdVotante = idUsuarioParticipante,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyectoPropio, IdCategoriaEvento = idCategoriaEvento },
                    new VotoDetalleDTO { IdProyecto = idProyectoAjeno1, IdCategoriaEvento = idCategoriaEvento },
                    new VotoDetalleDTO { IdProyecto = idProyectoAjeno2, IdCategoriaEvento = idCategoriaEvento }
                },
                IdEvento = idEvento
            };

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await votacionService.RegistrarVotoAsync(idVotacionPublicaSinAutovoto, dtoNoPermite);
            });

            var votosGuardadosNoPermite = await context.Voto
                .Where(v => v.Id_votacion == idVotacionPublicaSinAutovoto && v.Id_votante == idUsuarioParticipante)
                .ToListAsync();

            Assert.AreEqual(0, votosGuardadosNoPermite.Count, "No debe guardarse ningún voto si el autovoto está prohibido.");
        }

        [TestMethod]
        public async Task UsuarioConRolPublico_SoloPuedeVotarEnVotacionPublica()
        {
            var dto = new EmitirVotoDTO
            {
                IdVotante = idUsuarioPublico,
                Votos = new List<VotoDetalleDTO>
                {
                    new VotoDetalleDTO { IdProyecto = idProyectoPropio, IdCategoriaEvento = idCategoriaEvento },
                    new VotoDetalleDTO { IdProyecto = idProyectoAjeno1, IdCategoriaEvento = idCategoriaEvento },
                    new VotoDetalleDTO { IdProyecto = idProyectoAjeno2, IdCategoriaEvento = idCategoriaEvento }
                },
                IdEvento = idEvento
            };

            Assert.IsNotNull(dto.Votos);
            Assert.AreEqual(3, dto.Votos.Count);

            var resultadoPublica = await votacionService.RegistrarVotoAsync(idVotacionPublica, dto);
            Assert.IsTrue(resultadoPublica);
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await votacionService.RegistrarVotoAsync(idVotacionJurado, dto);
            });
            Assert.IsTrue(resultadoPublica, "Un usuario con rol público debe poder votar en una votación pública.");


            var votosPublica = await context.Voto
                .Where(v => v.Id_votacion == idVotacionPublica && v.Id_votante == idUsuarioPublico)
                .ToListAsync();

            var votosJurado = await context.Voto
                .Where(v => v.Id_votacion == idVotacionJurado && v.Id_votante == idUsuarioPublico)
                .ToListAsync();

            var votosMixta = await context.Voto
                .Where(v => v.Id_votacion == idVotacionMixta && v.Id_votante == idUsuarioPublico)
                .ToListAsync();

            Assert.AreEqual(3, votosPublica.Count);
            Assert.AreEqual(0, votosJurado.Count);
            Assert.AreEqual(0, votosMixta.Count);
        }
    }
}

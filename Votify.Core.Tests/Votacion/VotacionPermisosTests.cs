using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Factories;
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
        private VotacionService votacionService;

        private Guid idEvento;
        private Guid idUsuarioParticipante;
        private Guid idUsuarioPublico;

        private Guid idProyectoPropio;
        private Guid idProyectoAjeno1;
        private Guid idProyectoAjeno2;

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

            votacionService = new VotacionService(
                votacionRepository,
                votoRepository,
                proyectoRepository
            );

            idEvento = Guid.NewGuid();
            idUsuarioParticipante = Guid.NewGuid();
            idUsuarioPublico = Guid.NewGuid();

            idProyectoPropio = Guid.NewGuid();
            idProyectoAjeno1 = Guid.NewGuid();
            idProyectoAjeno2 = Guid.NewGuid();

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

            var evento = new Evento
            {
                Id = idEvento,
                Nombre = "Evento Test",
                Descripcion = "Evento para pruebas",
                FechaInicio = DateTime.Today.AddDays(-2),
                FechaFin = DateTime.Today.AddDays(2)
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

            var eventoUsuarioRolParticipante = new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idUsuarioParticipante,
                RolId = rolParticipante.Id
            };

            var eventoUsuarioRolPublico = new EventoUsuarioRol
            {
                EventoId = idEvento,
                UsuarioId = idUsuarioPublico,
                RolId = rolPublico.Id
            };

            var proyectoPropio = new Proyecto
            {
                Id = idProyectoPropio,
                Titulo = "Proyecto Propio",
                Descripcion = "Descripcion del proyecto propio",
                Id_evento = idEvento,
                Id_creador = idUsuarioParticipante
            };

            var proyectoAjeno1 = new Proyecto
            {
                Id = idProyectoAjeno1,
                Titulo = "Proyecto Ajeno 1",
                Descripcion = "Descripcion del proyecto ajeno 1",
                Id_evento = idEvento,
                Id_creador = Guid.NewGuid()
            };

            var proyectoAjeno2 = new Proyecto
            {
                Id = idProyectoAjeno2,
                Titulo = "Proyecto Ajeno 2",
                Descripcion = "Descripcion del proyecto ajeno 2",
                Id_evento = idEvento,
                Id_creador = Guid.NewGuid()
            };

            var ahora = DateTime.UtcNow;

            var votacionPublicaConAutovoto = new VotacionPublico
            {
                Id = idVotacionPublicaConAutovoto,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = true
            };

            var votacionPublicaSinAutovoto = new VotacionPublico
            {
                Id = idVotacionPublicaSinAutovoto,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = false
            };

            var votacionPublica = new VotacionPublico
            {
                Id = idVotacionPublica,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = true
            };

            var votacionJurado = new VotacionJurado
            {
                Id = idVotacionJurado,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = false
            };

            var votacionMixta = new VotacionMixta
            {
                Id = idVotacionMixta,
                Id_evento = idEvento,
                FechaInicio = ahora.AddHours(-1),
                FechaFin = ahora.AddHours(2),
                PermiteAutoVoto = false
            };

            context.Role.AddRange(rolParticipante, rolPublico);
            context.Usuario.AddRange(usuarioParticipante, usuarioPublico);
            context.Evento.Add(evento);
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
        new VotoDetalleDTO { IdProyecto = idProyectoPropio },
        new VotoDetalleDTO { IdProyecto = idProyectoAjeno1 },
        new VotoDetalleDTO { IdProyecto = idProyectoAjeno2 }
    }
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
        new VotoDetalleDTO { IdProyecto = idProyectoPropio },
        new VotoDetalleDTO { IdProyecto = idProyectoAjeno1 },
        new VotoDetalleDTO { IdProyecto = idProyectoAjeno2 }
    }
            };

            var resultadoNoPermite = await votacionService.RegistrarVotoAsync(idVotacionPublicaSinAutovoto, dtoNoPermite);

            Assert.IsFalse(resultadoNoPermite, "No debería permitir votar al propio proyecto cuando el autovoto no está permitido.");

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
            new VotoDetalleDTO { IdProyecto = idProyectoPropio },
            new VotoDetalleDTO { IdProyecto = idProyectoAjeno1 },
            new VotoDetalleDTO { IdProyecto = idProyectoAjeno2 }
        }
            };

            Assert.IsNotNull(dto.Votos);
            Assert.AreEqual(3, dto.Votos.Count);

            var resultadoPublica = await votacionService.RegistrarVotoAsync(idVotacionPublica, dto);
            var resultadoJurado = await votacionService.RegistrarVotoAsync(idVotacionJurado, dto);
            var resultadoMixta = await votacionService.RegistrarVotoAsync(idVotacionMixta, dto);

            Assert.IsTrue(resultadoPublica, "Un usuario con rol público debe poder votar en una votación pública.");
            Assert.IsFalse(resultadoJurado, "Un usuario con rol público no debe poder votar en una votación de jurado.");
            Assert.IsFalse(resultadoMixta, "Un usuario con rol público no debe poder votar en una votación mixta.");

            var votosPublica = await context.Voto
                .Where(v => v.Id_votacion == idVotacionPublica && v.Id_votante == idUsuarioPublico)
                .ToListAsync();

            var votosJurado = await context.Voto
                .Where(v => v.Id_votacion == idVotacionJurado && v.Id_votante == idUsuarioPublico)
                .ToListAsync();

            var votosMixta = await context.Voto
                .Where(v => v.Id_votacion == idVotacionMixta && v.Id_votante == idUsuarioPublico)
                .ToListAsync();

            Assert.AreEqual(3, votosPublica.Count, "En la votación pública sí deben guardarse votos.");
            Assert.AreEqual(0, votosJurado.Count, "En la votación de jurado no debe guardarse ningún voto.");
            Assert.AreEqual(0, votosMixta.Count, "En la votación mixta no debe guardarse ningún voto.");
        }
    }
}
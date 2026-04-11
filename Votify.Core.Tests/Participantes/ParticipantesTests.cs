using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Core.Services;
using Votify.Core.Entities;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

namespace Votify.Core.Tests.Participantes
{
    [TestClass]
    public class ParticipantesTests
    {
        private AppDbContext context;

        private Guid idRolPublico;
        private Guid idRolJurado;

        private Guid idUsuarioPublico;
        private Guid idUsuarioJurado;
        private Guid idUsuarioNoAsignado;

        private Guid idEventoPublico;
        private Guid idEventoJurado;
        private Guid idEventoMixto;
        private Guid idOtroEventoJurado;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new AppDbContext(options);

            idRolPublico = Guid.NewGuid();
            idRolJurado = Guid.NewGuid();

            var rolPublico = new Rol
            {
                Id = idRolPublico,
                Nombre = "Publico"
            };

            var rolJurado = new Rol
            {
                Id = idRolJurado,
                Nombre = "Jurado"
            };

            context.Role.AddRange(rolPublico, rolJurado);

            idUsuarioPublico = Guid.NewGuid();
            idUsuarioJurado = Guid.NewGuid();
            idUsuarioNoAsignado = Guid.NewGuid();

            context.Usuario.AddRange(
                new Usuario
                {
                    Id = idUsuarioPublico,
                    Nombre = "usuarioPublico",
                    Password = "1234",
                    Id_rol = idRolPublico,
                    Rol = rolPublico
                },
                new Usuario
                {
                    Id = idUsuarioJurado,
                    Nombre = "usuarioJurado",
                    Password = "1234",
                    Id_rol = idRolJurado,
                    Rol = rolJurado
                },
                new Usuario
                {
                    Id = idUsuarioNoAsignado,
                    Nombre = "usuarioNoAsignado",
                    Password = "1234",
                    Id_rol = idRolJurado,
                    Rol = rolJurado
                }
            );

            var hoy = DateOnly.FromDateTime(DateTime.Now);
            var categoriaId = Guid.NewGuid();

            context.Categoria.Add(new Categoria
            {
                Id = categoriaId,
                Nombre = "Categoria test"
            });

            idEventoPublico = Guid.NewGuid();
            idEventoJurado = Guid.NewGuid();
            idEventoMixto = Guid.NewGuid();
            idOtroEventoJurado = Guid.NewGuid();

            context.Evento.AddRange(
                new Evento
                {
                    Id = idEventoPublico,
                    Nombre = "Evento Publico",
                    Descripcion = "Evento publico test",
                    FechaInicio = hoy.AddDays(-1),
                    FechaFin = hoy.AddDays(1),
                    Id_categoria = categoriaId,
                    TipoVotacion = TipoVotacion.Publico
                },
                new Evento
                {
                    Id = idEventoJurado,
                    Nombre = "Evento Jurado",
                    Descripcion = "Evento jurado test",
                    FechaInicio = hoy.AddDays(-1),
                    FechaFin = hoy.AddDays(1),
                    Id_categoria = categoriaId,
                    TipoVotacion = TipoVotacion.Jurado
                },
                new Evento
                {
                    Id = idEventoMixto,
                    Nombre = "Evento Mixto",
                    Descripcion = "Evento mixto test",
                    FechaInicio = hoy.AddDays(-1),
                    FechaFin = hoy.AddDays(1),
                    Id_categoria = categoriaId,
                    TipoVotacion = TipoVotacion.Mixto
                },
                new Evento
                {
                    Id = idOtroEventoJurado,
                    Nombre = "Otro Evento Jurado",
                    Descripcion = "Otro evento jurado test",
                    FechaInicio = hoy.AddDays(-1),
                    FechaFin = hoy.AddDays(1),
                    Id_categoria = categoriaId,
                    TipoVotacion = TipoVotacion.Jurado
                }
            );

            context.EventoUsuarioRol.Add(new EventoUsuarioRol
            {
                Id = Guid.NewGuid(),
                EventoId = idEventoJurado,
                UsuarioId = idUsuarioJurado,
                RolId = idRolJurado
            });

            context.EventoUsuarioRol.Add(new EventoUsuarioRol
            {
                Id = Guid.NewGuid(),
                EventoId = idOtroEventoJurado,
                UsuarioId = idUsuarioNoAsignado,
                RolId = idRolJurado
            });

            context.SaveChanges();
        }

        [TestMethod]
        public async Task EventoPublico_UsuarioPublico_PuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioPublico);

            bool resultado = await PuedeVotarEnEventoAsync(idEventoPublico, usuario);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task EventoPublico_UsuarioNoPublico_NoPuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioJurado);

            bool resultado = await PuedeVotarEnEventoAsync(idEventoPublico, usuario);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task EventoJurado_UsuarioJurado_PuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioJurado);

            bool resultado = await PuedeVotarEnEventoAsync(idEventoJurado, usuario);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task EventoJurado_UsuarioNoJurado_NoPuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioNoAsignado);

            bool resultado = await PuedeVotarEnEventoAsync(idEventoJurado, usuario);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task EventoJurado_UsuarioJuradoOtroEvento_NoPuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioNoAsignado);

            bool resultado = await PuedeVotarEnEventoAsync(idEventoJurado, usuario);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task EventoJurado_UsuarioPublico_NoPuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioPublico);

            context.EventoUsuarioRol.Add(new EventoUsuarioRol
            {
                Id = Guid.NewGuid(),
                EventoId = idEventoJurado,
                UsuarioId = idUsuarioPublico,
                RolId = idRolPublico
            });

            await context.SaveChangesAsync();

            bool resultado = await PuedeVotarEnEventoAsync(idEventoJurado, usuario);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task EventoMixto_UsuarioPublico_PuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioPublico);

            bool resultado = await PuedeVotarEnEventoAsync(idEventoMixto, usuario);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task EventoMixto_UsuarioJurado_PuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioJurado);

            bool resultado = await PuedeVotarEnEventoAsync(idEventoMixto, usuario);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task UsuarioNull_NoPuedeVotar()
        {
            bool resultado = await PuedeVotarEnEventoAsync(idEventoPublico, null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task EventoNoExiste_NoPuedeVotar()
        {
            var usuario = await context.Usuario
                .Include(u => u.Rol)
                .FirstAsync(u => u.Id == idUsuarioPublico);

            bool resultado = await PuedeVotarEnEventoAsync(Guid.NewGuid(), usuario);

            Assert.IsFalse(resultado);
        }

        private async Task<bool> PuedeVotarEnEventoAsync(Guid idEvento, Usuario? usuario)
        {
            if (usuario == null)
            {
                return false;
            }

            var evento = await context.Evento.FirstOrDefaultAsync(e => e.Id == idEvento);

            if (evento == null)
            {
                return false;
            }

            var tipoEvento = evento.TipoVotacion;
            var rolUsuario = usuario.Rol?.Nombre;

            if (tipoEvento == TipoVotacion.Publico)
            {
                if (rolUsuario != "Publico")
                {
                    return false;
                }
            }

            if (tipoEvento == TipoVotacion.Jurado)
            {
                var autorizacion = await context.EventoUsuarioRol.AnyAsync(eur =>
                    eur.EventoId == idEvento &&
                    eur.UsuarioId == usuario.Id &&
                    eur.RolId == idRolJurado);

                if (!autorizacion)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

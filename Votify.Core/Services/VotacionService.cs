using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Factories;

namespace Votify.Core.Services
{
    public class VotacionService : IVotacion
    {
        private readonly IVotacionRepository votacionRepository;
        private readonly IVotoRepository votoRepository;
        private readonly IProyectoRepository proyectoRepository;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IEventoUsuarioRolRepository eventoUsuarioRolRepository;
        private readonly VotacionFactory votacionFactory;

        public VotacionService(IVotacionRepository votacionRepository, IVotoRepository votoRepository, IProyectoRepository proyectoRepository, IUsuarioRepository usuarioRepository , IEventoUsuarioRolRepository eventoUsuarioRolRepository)
        {
            this.votacionRepository = votacionRepository;
            this.votoRepository = votoRepository;
            this.proyectoRepository = proyectoRepository;
            this.usuarioRepository = usuarioRepository;
            this.eventoUsuarioRolRepository = eventoUsuarioRolRepository;
            this.votacionFactory = new ConcreteVotacionFactory();
        }
        public async Task<bool> YaVotoAsync(Guid idVotacion, Guid idVotante)
        {
            return await votacionRepository.YaVotoAsync(idVotacion, idVotante);
        }
        public async Task<List<Voto>> ObtenerVotosUsuarioAsync(Guid votacionId, Guid usuarioId)
        {
            return await votacionRepository.ObtenerPorVotacionYUsuarioAsync(votacionId, usuarioId);
        }

        public async Task<bool> RegistrarVotoAsync(Guid idVotacion, EmitirVotoDTO dto)
        {
            if (dto == null)
                throw new Exception("El DTO del voto es nulo.");

            if (dto.IdVotante == Guid.Empty)
                throw new Exception("El id del votante está vacío.");

            if (dto.Votos == null || dto.Votos.Count != 3)
                throw new Exception("Debes votar exactamente 3 proyectos.");

            var idsProyectos = dto.Votos.Select(v => v.IdProyecto).ToList();

            if (idsProyectos.Distinct().Count() != 3)
                throw new Exception("No puedes votar dos veces al mismo proyecto.");

            var votacion = await votacionRepository.ObtenerPorIdAsync(idVotacion);
            if (votacion == null)
                throw new Exception("La votación no existe.");

            var ahoraUtc = DateTime.UtcNow;
            if (ahoraUtc < votacion.FechaInicio || ahoraUtc > votacion.FechaFin)
                throw new Exception("La votación no está activa.");
            if (await YaVotoAsync(idVotacion, dto.IdVotante))
                throw new Exception("Ya has votado en esta votación.");

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(idsProyectos);
            if (proyectos.Count != 3)
                throw new Exception("No se han encontrado los 3 proyectos.");

            if (proyectos.Any(p => p.Id_evento != votacion.Id_evento))
                throw new Exception("Algún proyecto no pertenece al evento.");
            bool esPropio = proyectos.Any(p =>
                p.ProyectoParticipantes.Any(pp => pp.Id_usuario == dto.IdVotante)
            );
            if (!votacion.PermiteAutoVoto && esPropio)
                throw new Exception("No se permite el autovoto en esta votación.");


            Usuario usuario = await usuarioRepository.ObtenerPorIdAsync(dto.IdVotante);
            string? rolUsuario = usuario.Rol.Nombre;
            List<EventoUsuarioRol> eur = await eventoUsuarioRolRepository.ObtenerPorEventoYUsuario(dto.IdEvento, dto.IdVotante);

            bool esPublico = usuario.Rol.Nombre == "Publico" ||
                eur.Any(e => e.Rol.Nombre == "Publico" || e.Rol.Nombre == "Participante");

            bool esJuradoAsignado = eur.Any(e => e.Rol.Nombre == "Jurado");
            if (votacion is VotacionPublico)
            {
                if (!esPublico)
                {
                    throw new Exception("Solo usuarios públicos pueden votar en este evento.");
                }
            }
            else if (votacion is VotacionJurado)
            {

                if (!esJuradoAsignado)
                {
                    throw new Exception("Solo el jurado asignado puede votar.");
                }
            }
            else if (votacion is VotacionMixta)
            {
                if (!esPublico && !esJuradoAsignado)
                {
                    throw new Exception("Solo usuarios públicos, participantes o jurado asignado pueden votar en este evento.");
                }
            }

            var votos = dto.Votos.Select(v => new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_proyecto = v.IdProyecto,
                Id_votante = dto.IdVotante,
                Comentario = string.IsNullOrWhiteSpace(v.Comentario) ? null : v.Comentario.Trim(),
                Puntuacion = 1,
                Fecha_voto = DateTime.UtcNow
            }).ToList();

            await votoRepository.GuardarVotosAsync(votos);
            return true;
        }


        public async Task<List<Votacion>> BuscarVotacion(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas)
        {
            return await votacionRepository.BuscarPorNombreAsync(Id_evento, tipoVotacion, permiteAutoVoto, verFinalizadas);
        }

        public async Task CrearVotacion(Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion)
        {
            if (Id_evento == Guid.Empty)
                throw new Exception("El evento es obligatorio");

            if (finVotacion < inicioVotacion)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");

            var nuevaVotacion = votacionFactory.CrearVotacion(inicioVotacion, finVotacion, permiteAutoVoto,Id_evento,tipoVotacion);

            await votacionRepository.CrearVotacionAsync(nuevaVotacion);
        }
        public async Task EditarVotacion(Guid Id, Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion)
        {
            var votacionActual = await votacionRepository.ObtenerPorIdAsync(Id);

            if (votacionActual == null)
                throw new Exception("La votación no existe");
            System.Diagnostics.Debug.WriteLine($"Tipo actual real: {votacionActual.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"Tipo actual enum: {votacionActual.Tipo}");
            System.Diagnostics.Debug.WriteLine($"Tipo recibido: {tipoVotacion}");

            if (Id_evento == Guid.Empty)
                throw new Exception("El evento es obligatorio");

            if (finVotacion < inicioVotacion)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");

            if (votacionActual.Tipo == tipoVotacion)
            {
                votacionActual.Id_evento = Id_evento;
                votacionActual.FechaInicio = DateTime.SpecifyKind(inicioVotacion, DateTimeKind.Local).ToUniversalTime();
                votacionActual.FechaFin = DateTime.SpecifyKind(finVotacion, DateTimeKind.Local).ToUniversalTime();
                votacionActual.PermiteAutoVoto = permiteAutoVoto;

                await votacionRepository.EditarVotacionAsync(votacionActual);
                return;
            }

            var nuevaVotacion = votacionFactory.CrearVotacion(
                inicioVotacion,
                finVotacion,
                permiteAutoVoto,
                Id_evento,
                tipoVotacion
            );

            nuevaVotacion.Id = votacionActual.Id;

            await votacionRepository.ReemplazarVotacionAsync(votacionActual, nuevaVotacion);
        }

        public async Task EliminarVotacion(Guid Id)
        {
            var votacion = await votacionRepository.ObtenerPorIdAsync(Id);

            if (votacion == null)
                throw new Exception("La votación no existe");

            await votacionRepository.EliminarVotacionAsync(votacion);
        }

        public async Task<Votacion?> ObtenerPorIdAsync(Guid id)
        {
            return await votacionRepository.ObtenerPorIdAsync(id);
        }
        private static TipoVotacion ObtenerTipo(Votacion votacion)
        {
            if (votacion is VotacionPublico) return TipoVotacion.Publico;
            if (votacion is VotacionJurado) return TipoVotacion.Jurado;
            if (votacion is VotacionMixta) return TipoVotacion.Mixto;

            throw new Exception("Tipo de votación desconocido");
        }


    }
}
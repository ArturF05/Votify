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
        private readonly VotacionFactory votacionFactory;

        public VotacionService(IVotacionRepository votacionRepository, IVotoRepository votoRepository, IProyectoRepository proyectoRepository)
        {
            this.votacionRepository = votacionRepository;
            this.votoRepository = votoRepository;
            this.proyectoRepository = proyectoRepository;
            this.votacionFactory = new ConcreteVotacionFactory();
        }

        public async Task<bool> RegistrarVotoAsync(Guid idVotacion, EmitirVotoDTO dto)
        {
            if (dto == null)
                return false;
            if (dto.IdVotante == Guid.Empty)
                return false;

            if (dto.Votos == null || dto.Votos.Count != 3)
                return false;

            var idsProyectos = dto.Votos.Select(v => v.IdProyecto).ToList();

            if (idsProyectos.Distinct().Count() != 3)
                return false;

            var votacion = await votacionRepository.ObtenerPorIdAsync(idVotacion);
            if (votacion == null)
                return false;

            var hoy = DateTime.Now;
            if (hoy < votacion.FechaInicio || hoy > votacion.FechaFin)
                return false;

            bool yaVoto = await votoRepository.UsuarioYaVotoAsync(idVotacion, dto.IdVotante);
            if (yaVoto)
                return false;

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(idsProyectos);
            if (proyectos.Count != 3)
                return false;

            if (proyectos.Any(p => p.Id_evento != votacion.Id_evento))
                return false;

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
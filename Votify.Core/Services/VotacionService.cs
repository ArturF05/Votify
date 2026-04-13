using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class VotacionService : IVotacion
    {
        private readonly IVotacionRepository votacionRepository;
        private readonly IVotoRepository votoRepository;
        private readonly IProyectoRepository proyectoRepository;

        public VotacionService(IVotacionRepository votacionRepository, IVotoRepository votoRepository, IProyectoRepository proyectoRepository)
        {
            this.votacionRepository = votacionRepository;
            this.votoRepository = votoRepository;
            this.proyectoRepository = proyectoRepository;
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

            var hoy = DateTime.Now;
            if (hoy < votacion.FechaInicio || hoy > votacion.FechaFin)
                throw new Exception("La votación no está activa.");

            bool yaVoto = await votoRepository.UsuarioYaVotoAsync(idVotacion, dto.IdVotante);
            if (yaVoto)
                throw new Exception("Este usuario ya ha votado.");

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(idsProyectos);
            if (proyectos.Count != 3)
                throw new Exception("No se han encontrado los 3 proyectos.");

            if (proyectos.Any(p => p.Id_evento != votacion.Id_evento))
                throw new Exception("Algún proyecto no pertenece al evento.");

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

                var nuevaVotacion = new Votacion
                {
                    Id = Guid.NewGuid(),
                    Id_evento = Id_evento,
                    FechaInicio = inicioVotacion.ToUniversalTime(),
                    FechaFin = finVotacion.ToUniversalTime(),
                    PermiteAutoVoto = permiteAutoVoto,
                    TipoVotacion = tipoVotacion
                };

                await votacionRepository.CrearVotacionAsync(nuevaVotacion);
            }
        public async Task EditarVotacion(Guid Id, Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion)
        {
            var votacion = await votacionRepository.ObtenerPorIdAsync(Id);
            if (votacion == null)
                throw new Exception("La votación no existe");
            if (Id_evento == Guid.Empty)
                throw new Exception("El evento es obligatorio");
            if (finVotacion < inicioVotacion)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");
            votacion.Id_evento = Id_evento;
            votacion.FechaInicio = inicioVotacion.ToUniversalTime();
            votacion.FechaFin = finVotacion.ToUniversalTime();
            votacion.PermiteAutoVoto = permiteAutoVoto;
            votacion.TipoVotacion = tipoVotacion;

            await votacionRepository.EditarVotacionAsync(votacion);
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


    }
}
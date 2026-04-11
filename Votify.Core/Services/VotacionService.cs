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
            if (dto == null || dto.IdsProyectos == null) { return false; }
            if (dto.IdsProyectos.Count != 3) { return false; }
            if (dto.IdsProyectos.Distinct().Count() != 3) { return false; }

            var votacion = await votacionRepository.ObtenerPorIdAsync(idVotacion);
            if (votacion == null) { return false; }

            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (hoy < DateOnly.FromDateTime(votacion.FechaInicio) || hoy > DateOnly.FromDateTime(votacion.FechaFin))
            {
                return false;
            }

            bool yaVoto = await votoRepository.UsuarioYaVotoAsync(idVotacion, dto.IdVotante);
            if (yaVoto) { return false; }

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(dto.IdsProyectos);
            if (proyectos.Count != 3) { return false; }
            if (proyectos.Any(p => p.Id_evento != votacion.Evento.Id)) { return false; }

            var votos = dto.IdsProyectos.Select(idProyecto => new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_proyecto = idProyecto,
                Id_votante = dto.IdVotante,
                Puntuacion = 1,
                Fecha_voto = DateOnly.FromDateTime(DateTime.Now)
            }).ToList();

            await votoRepository.GuardarVotosAsync(votos);
            return true;
        }


        public async Task<List<Votacion>> BuscarVotacion(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas)
        {
            return await votacionRepository.BuscarPorNombreAsync(Id_evento, tipoVotacion, permiteAutoVoto, verFinalizadas);
        }
        
            public async Task CrearVotacion(Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto)
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
                };

                await votacionRepository.CrearVotacionAsync(nuevaVotacion);
            }
        public async Task EditarVotacion(Guid Id, Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto)
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
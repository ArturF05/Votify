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
            if(hoy < votacion.FechaInicio || hoy > votacion.FechaFin) { return false; }

            bool yaVoto = await votoRepository.UsuarioYaVotoAsync(dto.IdVotacion, dto.IdVotante)
            bool yaVoto = await votoRepository.UsuarioYaVotoAsync(idVotacion, dto.IdVotante);
            if(yaVoto) { return false; }

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(dto.IdsProyectos);
            if(proyectos.Count != 3) { return false; }
            if(proyectos.Any(p => p.Id_evento != votacion.Id_evento)) { return false; }

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
    }
}

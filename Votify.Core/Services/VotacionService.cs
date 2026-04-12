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
            if(dto == null || dto.Votos == null || dto.Votos.Count != 3) { return false; }

            var idsProyectos = dto.Votos.Select(v => v.IdProyecto).ToList();
            if (idsProyectos.Distinct().Count() != 3) { return false; }

            var votacion = await votacionRepository.ObtenerPorIdAsync(idVotacion);
            if (votacion == null) { return false; }

            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (hoy < DateOnly.FromDateTime(votacion.FechaInicio) ||hoy > DateOnly.FromDateTime(votacion.FechaFin))
            {
                return false;
            }

            bool yaVoto = await votoRepository.UsuarioYaVotoAsync(idVotacion, dto.IdVotante);
            if (yaVoto) { return false; }

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(idsProyectos);
            if (proyectos.Count != 3) { return false; }
            if (proyectos.Any(p => p.Id_evento != votacion.Id_evento)) { return false; }

            var votos = dto.Votos.Select(v => new Voto
            {
                Id = Guid.NewGuid(),
                Id_votacion = idVotacion,
                Id_proyecto = v.IdProyecto,
                Id_votante = dto.IdVotante,
                Comentario = v.Comentario,
                Puntuacion = 1,
                Fecha_voto =DateTime.UtcNow
            }).ToList();

            await votoRepository.GuardarVotosAsync(votos);
            return true;
        }
    }
}
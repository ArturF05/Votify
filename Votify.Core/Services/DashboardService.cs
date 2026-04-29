using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEventoRepository _eventoRepo;
        private readonly IVotacionRepository _votacionRepo;
        private readonly IProyectoRepository _proyectoRepo;
        private readonly IDimensionRepository _dimensionRepo;

        public DashboardService(
            IEventoRepository eventoRepo,
            IVotacionRepository votacionRepo,
            IProyectoRepository proyectoRepo,
            IDimensionRepository dimensionRepo)
        {
            _eventoRepo = eventoRepo;
            _votacionRepo = votacionRepo;
            _proyectoRepo = proyectoRepo;
            _dimensionRepo = dimensionRepo;
        }

        public async Task<DashboardEventoDTO?> ObtenerDashboardEvento(Guid eventoId)
        {
            var evento = await _eventoRepo.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                return null;

            var ranking = await ObtenerRankingProyectos(eventoId);
            var ganador = ranking.FirstOrDefault();

            var votacion = await _votacionRepo.ObtenerUltimaPorEventoAsync(eventoId);

            var criterios = votacion == null
                ? new List<CriterioDashboardDTO>()
                : (await _dimensionRepo.ObtenerCriterioPorVotacionAsync(votacion.Id))
                    .Select(d => new CriterioDashboardDTO
                    {
                        Id = d.Id,
                        Nombre = d.Nombre,
                        Descripcion = d.Descripcion ?? "",
                        Peso = d.Peso
                    })
                    .ToList();

            return new DashboardEventoDTO
            {
                EventoId = evento.Id,
                NombreEvento = evento.Nombre,

                ProyectoGanadorId = ganador?.ProyectoId ?? Guid.Empty,
                TituloProyectoGanador = ganador?.TituloProyecto ?? "Sin ganador",
                PuntuacionProyectoGanador = ganador?.PuntuacionTotal ?? 0,

                Proyectos = ranking,
                Criterios = criterios
            };
        }

        public async Task<List<ProyectoDashboardDTO>> ObtenerRankingProyectos(Guid eventoId)
        {
            var votacion = await _votacionRepo.ObtenerUltimaPorEventoAsync(eventoId);

            var proyectos = await _proyectoRepo.ObtenerPorEventoIdAsync(eventoId);

            if (proyectos == null || !proyectos.Any())
                return new List<ProyectoDashboardDTO>();

            if (votacion == null)
            {
                return proyectos.Select(p => new ProyectoDashboardDTO
                {
                    ProyectoId = p.Id,
                    TituloProyecto = p.Titulo,
                    PuntuacionTotal = 0,
                    Comentarios = new List<string>()
                }).ToList();
            }

            // 🔥 AQUÍ TU MODELO REAL
            var dimensiones = await _dimensionRepo.ObtenerCriterioPorVotacionAsync(votacion.Id);

            var votos = await _eventoRepo.ObtenerVotosDeEventoAsync(eventoId);

            var result = new List<ProyectoDashboardDTO>();

            foreach (var proyecto in proyectos)
            {
                decimal total = 0;

                foreach (var dim in dimensiones)
                {
                    var votosDim = votos
                        .Where(v => v.Id_proyecto == proyecto.Id && v.Id_categoria == dim.Id)
                        .ToList();

                    if (votosDim.Any())
                    {
                        var avg = votosDim.Average(v => v.Puntuacion);
                        total += avg * (dim.Peso / 100m);
                    }
                }

                result.Add(new ProyectoDashboardDTO
                {
                    ProyectoId = proyecto.Id,
                    TituloProyecto = proyecto.Titulo,
                    PuntuacionTotal = Math.Round(total, 2),
                    Comentarios = votos
                        .Where(v => v.Id_proyecto == proyecto.Id && !string.IsNullOrEmpty(v.Comentario))
                        .Select(v => v.Comentario!)
                        .ToList()
                });
            }

            return result
                .OrderByDescending(p => p.PuntuacionTotal)
                .ToList();
        }

        public async Task<ProyectoDashboardDTO?> ObtenerProyectoGanador(Guid eventoId)
        {
            var ranking = await ObtenerRankingProyectos(eventoId);
            return ranking.FirstOrDefault();
        }

        public async Task<List<CriterioDashboardDTO>> ObtenerCriteriosVotacion(Guid eventoId)
        {
            var votacion = await _votacionRepo.ObtenerUltimaPorEventoAsync(eventoId);

            if (votacion == null)
                return new List<CriterioDashboardDTO>();

            return (await _dimensionRepo.ObtenerCriterioPorVotacionAsync(votacion.Id))
                .Select(d => new CriterioDashboardDTO
                {
                    Id = d.Id,
                    Nombre = d.Nombre,
                    Descripcion = d.Descripcion ?? "",
                    Peso = d.Peso
                })
                .ToList();
        }
    }
}
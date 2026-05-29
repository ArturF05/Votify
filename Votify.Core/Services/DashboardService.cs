using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEventoRepository _eventoRepo;
        private readonly IVotacionRepository _votacionRepo;
        private readonly IProyectoRepository _proyectoRepo;
        private readonly IDimensionRepository _dimensionRepo;

        private const decimal PORCENTAJE_TOTAL = 100m;
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
                        CriterioId   = d.Id,
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

            var dimensiones = votacion == null
                ? new List<CriterioDashboardDTO>()
                : votacion.DimensionesDeValores
                    .Select(d => new CriterioDashboardDTO
                    {
                        CriterioId = d.Id,
                        Nombre = d.Nombre,
                        Descripcion = d.Descripcion ?? "",
                        Peso = d.Peso
                    })
                    .ToList();
            var votos = await _eventoRepo.ObtenerVotosDeEventoAsync(eventoId);

            var result = new List<ProyectoDashboardDTO>();

            foreach (var proyecto in proyectos)
            {
                decimal total = 0;

                foreach (var dim in dimensiones)
                {
                    var votosDim = votos
                        .Where(v => v.Id_proyecto == proyecto.Id && v.VotosCriterio.Any(vc => vc.Id_criterio == dim.CriterioId))
                        .ToList();

                    if (votosDim.Any())
                    {
                        var avg = (decimal)(votosDim
                            .SelectMany(v => v.VotosCriterio)
                            .Where(vc => vc.Id_criterio == dim.CriterioId)
                            .Average(vc => vc.Nota));
                        total += avg * (dim.Peso / PORCENTAJE_TOTAL);
                    }
                }

                var criteriosProyecto = dimensiones.Select(dim =>
                {
                    var votosCriterio = votos
                        .Where(v => v.Id_proyecto == proyecto.Id)
                        .SelectMany(v => v.VotosCriterio)
                        .Where(vc => vc.Id_criterio == dim.CriterioId)
                        .ToList();

                    return new CriterioDashboardDTO
                    {
                        CriterioId = dim.CriterioId,
                        Nombre = dim.Nombre,
                        Descripcion = dim.Descripcion,
                        Peso = dim.Peso,
                        PuntuacionMedia = votosCriterio.Any()
                            ? (decimal)votosCriterio.Average(vc => vc.Nota)
                            : 0m,
                        Comentarios = votosCriterio
                            .Where(vc => !string.IsNullOrWhiteSpace(vc.Comentario))
                            .Select(vc => $"{ObtenerAutorComentario(vc.Voto)}: {vc.Comentario!}")
                            .ToList()
                    };
                }).ToList();

                result.Add(new ProyectoDashboardDTO
                {
                    ProyectoId = proyecto.Id,
                    TituloProyecto = proyecto.Titulo,
                    PuntuacionTotal = Math.Round(total, 2),
                    Comentarios = votos
                        .Where(v => v.Id_proyecto == proyecto.Id && !string.IsNullOrEmpty(v.Comentario))
                        .Select(v => $"{ObtenerAutorComentario(v)}: {v.Comentario!}")
                        .ToList(),
                    Criterios = criteriosProyecto
                });
            }


            return result
                .OrderByDescending(p => p.PuntuacionTotal)
                .ToList();
        }

        private static string ObtenerAutorComentario(Voto voto)
        {
            if (voto.Votacion?.ComentariosAnonimos == true)
                return "Anónimo";

            return string.IsNullOrWhiteSpace(voto.Usuario?.Nombre)
                ? "Juez"
                : voto.Usuario.Nombre;
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
                    CriterioId = d.Id,
                    Nombre = d.Nombre,
                    Descripcion = d.Descripcion ?? "",
                    Peso = d.Peso
                })
                .ToList();
        }

    }
}

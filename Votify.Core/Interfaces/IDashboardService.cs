using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.DTOs;

namespace Votify.Core.Services
{
    public interface IDashboardService
    {
        Task<DashboardEventoDTO?> ObtenerDashboardEvento(Guid eventoId);
        Task<List<CriterioDashboardDTO>> ObtenerCriteriosVotacion(Guid eventoId);
        Task<List<ProyectoDashboardDTO>> ObtenerRankingProyectos(Guid eventoId);
        Task<ProyectoDashboardDTO?> ObtenerProyectoGanador(Guid eventoId);


        
    }
}
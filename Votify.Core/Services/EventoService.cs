using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Factories;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class EventoService : IEvento
    {
        /*private Rol aux = new Rol();
        private List<Proyecto> aux2 = new List<Proyecto>();
        private Votify.Core.Entities.Votacion aux3 = new Votify.Core.Entities.Votacion();*/
        private readonly IEventoRepository eventoRepository;
        private readonly EventoFactory eventoFactory = new ConcreteEventFactory();
        public EventoService(IEventoRepository eventoRepository)
        {
            this.eventoRepository = eventoRepository;
        }
        public async Task<List<Evento>?> BuscarEvento(string nombre, Guid Id_Categoria, Guid idUsuario)
        {
            return await eventoRepository.BuscarPorNombreAsync(nombre, Id_Categoria, idUsuario);
        }
        public async Task<Evento> CrearEvento(string titulo, Guid categoriaId, DateTime fechaInicio, string descripcion, DateTime fechaFin,string tipo, Guid usuarioId, string? imagenUrl)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new Exception("El título es obligatorio");

            if (fechaFin < fechaInicio)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");

            Evento nuevoEvento = eventoFactory.CreateEvento(tipo, titulo, descripcion, fechaInicio, fechaFin, categoriaId);
            nuevoEvento.ImagenUrl = imagenUrl;
            var aux = await eventoRepository.CrearEventoAsync(nuevoEvento);
            await eventoRepository.AsignarOrganizadorAsync(nuevoEvento.Id, usuarioId);
            return aux;


        }

        public async Task EditarEvento(Guid eventoId, string titulo, Guid categoriaId, DateTime fechaInicio, string descripcion, DateTime fechaFin, string? imagenUrl)
        {
            var evento = await eventoRepository.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            if (string.IsNullOrWhiteSpace(titulo))
                throw new Exception("El título es obligatorio");

            if (fechaFin < fechaInicio)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");

            evento.Nombre = titulo;
            evento.Id_categoria = categoriaId;
            evento.Descripcion = descripcion;
            evento.FechaInicio = DateTime.SpecifyKind(fechaInicio, DateTimeKind.Utc);
            evento.FechaFin = DateTime.SpecifyKind(fechaFin, DateTimeKind.Utc);
            evento.ImagenUrl = imagenUrl;

            await eventoRepository.EditarEventoAsync(evento);
        }

        public async Task EliminarEvento(Guid eventoId)
        {
            var evento = await eventoRepository.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            await eventoRepository.EliminarEventoAsync(evento);
        }
        public async Task Archivo(Guid eventoId, bool archivar)
        {
            var evento = await eventoRepository.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            await eventoRepository.ArchivoEventoAsync(evento, archivar);
        }

        public async Task ApuntarseAsync(Guid eventoId, Guid usuarioId, bool apuntar)
        {
            var evento = await eventoRepository.ObtenerEventoPorIdAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            await eventoRepository.ApuntarseEventoAsync(evento, usuarioId, apuntar);
        }

        public async Task<Evento?> ObtenerEventoPorIdAsync(Guid id)
        {
            return await eventoRepository.ObtenerEventoPorIdAsync(id);
        }
        public async Task<List<Evento>> ObtenerTodos()
        {
            return await eventoRepository.ObtenerTodos();
        }
        public async Task<int> NumeroVotosPorEvento(Guid idEvento)
        {
            var  votos = await eventoRepository.ObtenerVotosDeEventoAsync(idEvento);
            return votos.Count();
        }
        public async Task<List<Votify.Core.Entities.Evento>> ObtenerEventosOrganizador(Guid idUsuario)
        {
            return await eventoRepository.ObtenerEventosOrganizadorAsync(idUsuario);
        }

        public async Task<DashboardEventoDTO> ObtenerDashboardEventoAsync(Guid eventoId)
        {
            var evento = await eventoRepository.ObtenerEventoDashboardAsync(eventoId);

            if (evento == null)
                throw new Exception("El evento no existe");

            var votacion = await eventoRepository.ObtenerVotacionPorEventoAsync(eventoId);

            if (votacion == null)
                throw new Exception("Este evento no tiene votación");

            var hoy = DateTime.UtcNow;

            if (votacion.FechaFin > hoy)
                throw new Exception("Todavía no se puede acceder al dashboard porque la votación no ha finalizado");

            var votos = await eventoRepository.ObtenerVotosDeEventoAsync(eventoId);

            if (evento.Proyectos == null || evento.Proyectos.Count == 0)
                throw new Exception("El evento no tiene proyectos");

            var proyectosDashboard = evento.Proyectos
                .Select(p => new ProyectoDashboardDTO
                {
                    ProyectoId = p.Id,
                    TituloProyecto = p.Titulo,
                    PuntuacionTotal = votos
                        .Where(v => v.Id_proyecto == p.Id)
                        .Sum(v => v.Puntuacion),
                    Comentarios = votos
                        .Where(v => v.Id_proyecto == p.Id && !string.IsNullOrWhiteSpace(v.Comentario))
                        .Select(v => v.Comentario!)
                        .ToList(),
                    Criterios = votacion.DimensionesDeValores
                        .OrderBy(c => c.Nombre)
                        .Select(c =>
                        {
                            var votosCriterio = votos
                                .Where(v => v.Id_proyecto == p.Id)
                                .SelectMany(v => v.VotosCriterio)
                                .Where(vc => vc.Id_criterio == c.Id)
                                .ToList();

                            return new CriterioDashboardDTO
                            {
                                CriterioId = c.Id,
                                Nombre = c.Nombre,
                                Descripcion = c.Descripcion,
                                Peso = c.Peso,
                                PuntuacionMedia = votosCriterio.Any()
                                    ? (decimal)votosCriterio.Average(vc => vc.Nota)
                                    : 0m,
                                Comentarios = votosCriterio
                                    .Where(vc => !string.IsNullOrWhiteSpace(vc.Comentario))
                                    .Select(vc => vc.Comentario!)
                                    .ToList()
                            };
                        })
                        .ToList()
                })
                .OrderByDescending(p => p.PuntuacionTotal)
                .ToList();

            var ganador = proyectosDashboard.First();

            return new DashboardEventoDTO
            {
                EventoId = evento.Id,
                NombreEvento = evento.Nombre,
                ProyectoGanadorId = ganador.ProyectoId,
                TituloProyectoGanador = ganador.TituloProyecto,
                PuntuacionProyectoGanador = ganador.PuntuacionTotal,
                Criterios = votacion.DimensionesDeValores
                    .OrderBy(c => c.Nombre)
                    .Select(c => new CriterioDashboardDTO
                    {
                        CriterioId = c.Id,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        Peso = c.Peso,
                        PuntuacionMedia = votos
                            .SelectMany(v => v.VotosCriterio)
                            .Where(vc => vc.Id_criterio == c.Id)
                            .DefaultIfEmpty()
                            .Average(vc => vc == null ? 0m : vc.Nota)
                    })
                    .ToList(),
                Proyectos = proyectosDashboard
            };
        }
    }
}

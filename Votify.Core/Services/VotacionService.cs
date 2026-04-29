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
        private readonly IDimensionRepository dimensionRepository;
        private readonly VotacionFactory votacionFactory;

        public VotacionService(IVotacionRepository votacionRepository, IVotoRepository votoRepository, IProyectoRepository proyectoRepository, IDimensionRepository dimensionRepository, IUsuarioRepository usuarioRepository , IEventoUsuarioRolRepository eventoUsuarioRolRepository)
        {
            this.votacionRepository = votacionRepository;
            this.votoRepository = votoRepository;
            this.proyectoRepository = proyectoRepository;
            this.dimensionRepository = dimensionRepository;
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
                throw new Exception("Error interno: El DTO del voto es nulo.");

            if (dto.IdVotante == Guid.Empty)
                throw new Exception("Error interno: El id del votante está vacío.");

            var votacion = await votacionRepository.ObtenerPorIdAsync(idVotacion);
            if (votacion == null)
                throw new Exception("Error interno: La votación no existe.");

            int numProy = votacion.NumProy;

            if (dto.Votos == null)
                throw new Exception($"Debes votar exactamente {numProy} proyectos.");

            if (dto.Votos.Count > numProy)
                throw new Exception("No puedes votar más proyectos que el máximo permitido.");

            var idsProyectos = dto.Votos.Select(v => v.IdProyecto).ToList();

            if (idsProyectos.Distinct().Count() != idsProyectos.Count)
                throw new Exception("No puedes votar dos veces al mismo proyecto.");

            var ahoraUtc = DateTime.UtcNow;
            if (ahoraUtc < votacion.FechaInicio || ahoraUtc > votacion.FechaFin)
                throw new Exception("La votación no está activa.");
            if (await YaVotoAsync(idVotacion, dto.IdVotante))
                throw new Exception("Ya has votado en esta votación.");

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(idsProyectos);

            if (proyectos.Count != idsProyectos.Count)
                throw new Exception($"No se han encontrado los {numProy} proyectos que debías votar.");

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

            var criterios = await dimensionRepository.ObtenerCriterioPorVotacionAsync(idVotacion);

            if (criterios == null || criterios.Count == 0)
                throw new Exception("La votación no tiene criterios definidos.");
            var votos = dto.Votos.Select(v =>
            {
                decimal puntuacionTotal = 0m;

                if (v.Puntuaciones == null || !v.Puntuaciones.Any())
                {
                    puntuacionTotal = 1m;
                } 
                else
                {
                    foreach (var criterio in criterios)
                    {
                        var puntuacion = v.Puntuaciones
                            .FirstOrDefault(p => p.IdCriterio == criterio.Id);

                        if (puntuacion == null)
                            throw new Exception("Falta la puntuación de algún criterio.");

                        puntuacionTotal += puntuacion.Nota * (criterio.Peso / 100m);
                    }
                }
                if (votacion.PermiteComentarios && votacion.ComentariosObligatorios && (string.IsNullOrEmpty(v.Comentario) || string.IsNullOrWhiteSpace(v.Comentario)))
                    throw new Exception("En esta votación, es obligatorio incluir un comentario general en cada voto");
                return new Voto
                {
                    Id = Guid.NewGuid(),
                    Id_votacion = idVotacion,
                    Id_proyecto = v.IdProyecto,
                    Id_votante = dto.IdVotante,
                    Comentario = string.IsNullOrWhiteSpace(v.Comentario) ? null : v.Comentario.Trim(),
                    Puntuacion = puntuacionTotal,
                    Fecha_voto = DateTime.UtcNow
                };
            }).ToList();

            await votoRepository.GuardarVotosAsync(votos);
            return true;
        }


        public async Task<List<Votacion>> BuscarVotacion(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas)
        {
            return await votacionRepository.BuscarPorNombreAsync(Id_evento, tipoVotacion, permiteAutoVoto, verFinalizadas);
        }

        public async Task CrearVotacion(Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion,int NumProy, bool permiteComentarios, bool comentariosObligatorios)
        {
            if (Id_evento == Guid.Empty)
                throw new Exception("El evento es obligatorio");

            if (finVotacion < inicioVotacion)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");
            if (NumProy < 1)
                throw new Exception("Se tiene que poder votar mínimo 1 proyecto");
            if (permiteComentarios == false) comentariosObligatorios = false;
            var nuevaVotacion = votacionFactory.CrearVotacion(inicioVotacion, finVotacion, permiteAutoVoto,Id_evento,tipoVotacion,NumProy, permiteComentarios, comentariosObligatorios);

            await votacionRepository.CrearVotacionAsync(nuevaVotacion);
        }
        public async Task EditarVotacion(Guid Id, Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion, List<CrearCriterioDTO> criterios,int NumProy, bool permiteComentarios, bool comentariosObligatorios)
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
            if (!permiteComentarios && comentariosObligatorios) comentariosObligatorios = false;
            if (votacionActual.Tipo == tipoVotacion)
            {
                votacionActual.Id_evento = Id_evento;
                votacionActual.FechaInicio = DateTime.SpecifyKind(inicioVotacion, DateTimeKind.Local).ToUniversalTime();
                votacionActual.FechaFin = DateTime.SpecifyKind(finVotacion, DateTimeKind.Local).ToUniversalTime();
                votacionActual.PermiteAutoVoto = permiteAutoVoto;
                votacionActual.NumProy = NumProy;
                votacionActual.PermiteComentarios = permiteComentarios;
                votacionActual.ComentariosObligatorios = comentariosObligatorios;
                await votacionRepository.EditarVotacionAsync(votacionActual);
            }
            else
            {
                var nuevaVotacion = votacionFactory.CrearVotacion(
                    inicioVotacion,
                    finVotacion,
                    permiteAutoVoto,
                    Id_evento,
                    tipoVotacion,
                    NumProy,
                    permiteComentarios,
                    comentariosObligatorios
                );

                nuevaVotacion.Id = votacionActual.Id;

                await votacionRepository.ReemplazarVotacionAsync(votacionActual, nuevaVotacion);
            }

            var criteriosExistentes = await dimensionRepository.ObtenerCriterioPorVotacionAsync(Id);

            foreach (var criterioDto in criterios)
            {
                var existente = criteriosExistentes.FirstOrDefault(c => c.Id == criterioDto.Id);

                if (existente == null)
                {
                    var nuevoCriterio = new DimensionesDeValor
                    {
                        Id = criterioDto.Id == Guid.Empty ? Guid.NewGuid() : criterioDto.Id,
                        Id_votacion = Id,
                        Nombre = criterioDto.Nombre,
                        Descripcion = criterioDto.Descripcion,
                        Peso = criterioDto.Peso
                    };

                    await dimensionRepository.CrearCriterioAsync(nuevoCriterio);
                }
                else
                {
                    existente.Peso = criterioDto.Peso;
                    await dimensionRepository.EditarCriterioAsync(existente);
                }
            }

            var criteriosABorrar = criteriosExistentes
                .Where(c => !criterios.Any(dto => dto.Id == c.Id))
                .ToList();

            foreach (var criterio in criteriosABorrar)
            {
                await dimensionRepository.EliminarCriterioAsync(criterio);
            }
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
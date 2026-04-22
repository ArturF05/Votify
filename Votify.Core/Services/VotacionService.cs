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
        private readonly IDimensionRepository dimensionRepository;
        private readonly VotacionFactory votacionFactory;

        public VotacionService(IVotacionRepository votacionRepository, IVotoRepository votoRepository, IProyectoRepository proyectoRepository, IDimensionRepository dimensionRepository)
        {
            this.votacionRepository = votacionRepository;
            this.votoRepository = votoRepository;
            this.proyectoRepository = proyectoRepository;
            this.dimensionRepository = dimensionRepository;
            this.votacionFactory = new ConcreteVotacionFactory();
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

            var ahoraUtc = DateTime.UtcNow;
            if (ahoraUtc < votacion.FechaInicio || ahoraUtc > votacion.FechaFin)
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

        public async Task CrearVotacion(Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion,int NumProy)
        {
            if (Id_evento == Guid.Empty)
                throw new Exception("El evento es obligatorio");

            if (finVotacion < inicioVotacion)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");

            var nuevaVotacion = votacionFactory.CrearVotacion(inicioVotacion, finVotacion, permiteAutoVoto,Id_evento,tipoVotacion,NumProy);

            await votacionRepository.CrearVotacionAsync(nuevaVotacion);
        }
        public async Task EditarVotacion(Guid Id, Guid Id_evento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion, List<CrearCriterioDTO> criterios,int NumProy)
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
            }
            else
            {
                var nuevaVotacion = votacionFactory.CrearVotacion(
                    inicioVotacion,
                    finVotacion,
                    permiteAutoVoto,
                    Id_evento,
                    tipoVotacion,
                    NumProy
                );

                nuevaVotacion.Id = votacionActual.Id;

                await votacionRepository.ReemplazarVotacionAsync(votacionActual, nuevaVotacion);
            }

            var criteriosExistentes = await dimensionRepository.ObtenerCriterioPorVotacionAsync(Id);

            foreach (var criterioDto in criterios)
            {
                bool yaExiste = criteriosExistentes.Any(c => c.Id == criterioDto.Id);

                if (!yaExiste)
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
            }

            var idsEnListaActual = criterios.Select(c => c.Id).ToHashSet();

            var criteriosABorrar = criteriosExistentes
                .Where(c => !idsEnListaActual.Contains(c.Id))
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
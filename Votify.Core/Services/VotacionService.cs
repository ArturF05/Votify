using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Factories;
using Votify.Core.Observer;
using Votify.Core.State;

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
        private readonly EstadoVotacionFactory estadoVotacionFactory;
        private readonly IEmailSender emailSender;

        private const decimal PUNTUACION_DEFECTO = 1m;
        private const decimal PORCENTAJE_TOTAL = 100m;

        public VotacionService(IVotacionRepository votacionRepository, IVotoRepository votoRepository, IProyectoRepository proyectoRepository, IDimensionRepository dimensionRepository, IUsuarioRepository usuarioRepository, IEventoUsuarioRolRepository eventoUsuarioRolRepository, IEmailSender emailSender)
        {
            this.votacionRepository = votacionRepository;
            this.votoRepository = votoRepository;
            this.proyectoRepository = proyectoRepository;
            this.dimensionRepository = dimensionRepository;
            this.usuarioRepository = usuarioRepository;
            this.eventoUsuarioRolRepository = eventoUsuarioRolRepository;
            this.emailSender = emailSender;
            this.votacionFactory = new ConcreteVotacionFactory();
            this.estadoVotacionFactory = new EstadoVotacionFactory();
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
            Votacion votacion = await ValidarVotacionYDtoAsync(idVotacion, dto);
            List<Proyecto> proyectos = await ValidarProyectosAsync(votacion, dto);
            await ValidarPermisosVotante(votacion, dto);
            return await ConstruirVotosAsync(idVotacion, votacion, dto, proyectos);
        }
        private async Task<Votacion> ValidarVotacionYDtoAsync(Guid idVotacion, EmitirVotoDTO dto)
        {
            if (dto == null)
                throw new Exception("El DTO del voto es nulo.");

            if (dto.IdVotante == Guid.Empty)
                throw new Exception("El id del votante está vacío.");

            var votacion = await votacionRepository.ObtenerPorIdAsync(idVotacion);
            if (votacion == null)
                throw new Exception("La votación no existe.");

            var ahoraUtc = DateTime.UtcNow;
            if (ahoraUtc < votacion.FechaInicio || ahoraUtc > votacion.FechaFin)
                throw new Exception("La votación no está activa.");

            return votacion;
        }
        private async Task<List<Proyecto>> ValidarProyectosAsync(Votacion votacion, EmitirVotoDTO dto)
        {
            if (dto.Votos == null || dto.Votos.Count == 0)
                throw new Exception($"Debes votar hasta {votacion.NumProy} proyectos.");

            var idsProyectos = dto.Votos.Select(v => v.IdProyecto).ToList();

            if (idsProyectos.Distinct().Count() != idsProyectos.Count)
                throw new Exception("No puedes votar dos veces al mismo proyecto.");

            if (dto.Votos.Count > votacion.NumProy)
                throw new Exception("No puedes votar más proyectos que el máximo permitido.");

            var proyectos = await proyectoRepository.ObtenerPorIdAsync(idsProyectos);

            if (proyectos.Count != idsProyectos.Count)
                throw new Exception($"No se han encontrado los {votacion.NumProy} proyectos que debías votar.");

            if (proyectos.Any(p => p.Id_evento != votacion.Id_evento))
                throw new Exception("Algún proyecto no pertenece al evento.");

            bool esPropio = proyectos.Any(p =>
                p.ProyectoParticipantes.Any(pp => pp.Id_usuario == dto.IdVotante));

            bool autoVotoBloqueado = !votacion.PermiteAutoVoto && esPropio;
            if (autoVotoBloqueado)
                throw new Exception("No se permite el autovoto en esta votación.");

            return proyectos;
        }
        private async Task ValidarPermisosVotante(Votacion votacion, EmitirVotoDTO dto)
        {
            var usuario = await usuarioRepository.ObtenerPorIdAsync(dto.IdVotante);
            var eur = await eventoUsuarioRolRepository.ObtenerPorEventoYUsuario(dto.IdEvento, dto.IdVotante);

            bool esPublico = usuario.Rol.Nombre == "Publico" ||
                eur.Any(e => e.Rol.Nombre == "Publico" || e.Rol.Nombre == "Participante");

            bool esJuradoAsignado = eur.Any(e => e.Rol.Nombre == "Jurado");

            if (votacion is VotacionPublico && !esPublico)
                throw new Exception("Solo usuarios públicos pueden votar en este evento.");

            if (votacion is VotacionJurado && !esJuradoAsignado)
                throw new Exception("Solo el jurado asignado puede votar.");

            if (votacion is VotacionMixta && !esPublico && !esJuradoAsignado)
                throw new Exception("Solo usuarios públicos, participantes o jurado asignado pueden votar en este evento.");
        }
        public async Task<bool> ConstruirVotosAsync(Guid idVotacion,Votacion votacion,EmitirVotoDTO dto,List<Proyecto> proyectos)
        {
            var ahoraUtc = DateTime.UtcNow;
            var estado = estadoVotacionFactory.Crear(votacion.EstadoVotacion);
            estado.ValidarPuedeVotar(votacion, ahoraUtc);
            int numProy = votacion.NumProy;
            var votos = new List<Voto>();
            foreach (var v in dto.Votos)
            {
                if (v.IdCategoriaEvento == Guid.Empty)
                    throw new Exception("Debes indicar la categoría del evento que estás votando.");

                if (!await dimensionRepository.CategoriaEventoPerteneceAlEventoAsync(v.IdCategoriaEvento, votacion.Id_evento))
                    throw new Exception("La categoría seleccionada no pertenece al evento de la votación.");

                if (votacion.Id_categoria_evento.HasValue && votacion.Id_categoria_evento.Value != v.IdCategoriaEvento)
                    throw new Exception("Esta votación no corresponde a la categoría seleccionada.");

                var proyectoVotado = proyectos.FirstOrDefault(p => p.Id == v.IdProyecto);
                if (proyectoVotado?.Id_categoria_evento != v.IdCategoriaEvento)
                    throw new Exception("El proyecto no pertenece a la categoría seleccionada.");

                if (await votacionRepository.YaVotoCategoriaAsync(idVotacion, dto.IdVotante, v.IdCategoriaEvento))
                    throw new Exception("Ya has votado en esta categoría.");

                var criterios = await dimensionRepository.ObtenerCriteriosPorCategoriaEventoAsync(idVotacion, v.IdCategoriaEvento);

                if (criterios == null || criterios.Count == 0)
                    throw new Exception("La categoría seleccionada no tiene criterios definidos.");

                var idVoto = Guid.NewGuid();
                decimal puntuacionTotal = 0m;
                var voto = new Voto
                {
                    Id = idVoto,
                    Id_votacion = idVotacion,
                    Id_proyecto = v.IdProyecto,
                    Id_votante = dto.IdVotante,
                    Id_categoria = v.IdCategoriaEvento,
                    Fecha_voto = DateTime.UtcNow
                };

                if (v.Puntuaciones == null || !v.Puntuaciones.Any())
                {
                    puntuacionTotal = PUNTUACION_DEFECTO;
                } 
                else
                {
                    foreach (var criterio in criterios)
                    {
                        var puntuacion = v.Puntuaciones
                            .FirstOrDefault(p => p.IdCriterio == criterio.Id);

                        if (puntuacion == null)
                            throw new Exception("Falta la puntuación de algún criterio.");
                        bool comentarioEsObligatorio = votacion.PermiteComentarios && votacion.ComentariosObligatorios;
                        bool comentarioFaltante = string.IsNullOrWhiteSpace(puntuacion.Comentario);
                        if (comentarioEsObligatorio && comentarioFaltante)
                            throw new Exception("En esta votación, es obligatorio incluir un comentario por cada criterio en cada voto");
                        puntuacionTotal += puntuacion.Nota * (criterio.Peso / PORCENTAJE_TOTAL);
                        voto.VotosCriterio.Add(new VotoCriterio
                        {
                            Id = Guid.NewGuid(),
                            Id_voto = idVoto,
                            Id_criterio = criterio.Id,
                            Nota = puntuacion.Nota,
                            Voto = voto,
                            Comentario = string.IsNullOrWhiteSpace(puntuacion.Comentario)
                                ? null
                                : puntuacion.Comentario.Trim()
                        });
                    }
                }

                voto.Puntuacion = puntuacionTotal;
                votos.Add(voto);
            }

            await votoRepository.GuardarVotosAsync(votos);
            return true;
        }


        public async Task<List<Votacion>> BuscarVotacion(Guid Id_evento, int tipoVotacion, int permiteAutoVoto, bool verFinalizadas)
        {
            return await votacionRepository.BuscarPorNombreAsync(Id_evento, tipoVotacion, permiteAutoVoto, verFinalizadas);
        }

        public async Task CrearVotacion(Guid Id_evento, Guid? idCategoriaEvento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion,int NumProy, bool permiteComentarios, bool comentariosObligatorios, bool comentariosAnonimos, bool verRes,List<Guid> Jurados, EstadoVotacion estadoVotacion, List<CrearCriterioDTO> criterios)
        {
            if (Id_evento == Guid.Empty)
                throw new Exception("El evento es obligatorio");

            if (finVotacion < inicioVotacion)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");
            if (NumProy < 1)
                throw new Exception("Se tiene que poder votar mínimo 1 proyecto");
            if (idCategoriaEvento.HasValue && idCategoriaEvento.Value != Guid.Empty &&
                !await dimensionRepository.CategoriaEventoPerteneceAlEventoAsync(idCategoriaEvento.Value, Id_evento))
                throw new Exception("La categoría de la votación no pertenece al evento");
            (comentariosObligatorios, comentariosAnonimos) = NormalizarOpcionesComentarios(
                permiteComentarios, comentariosObligatorios, comentariosAnonimos);
            var nuevaVotacion = votacionFactory.CrearVotacion(inicioVotacion, finVotacion, permiteAutoVoto,Id_evento,tipoVotacion,NumProy, permiteComentarios, comentariosObligatorios, comentariosAnonimos, verRes,estadoVotacion);
            nuevaVotacion.Id_categoria_evento = idCategoriaEvento == Guid.Empty ? null : idCategoriaEvento;

            await votacionRepository.CrearVotacionAsync(nuevaVotacion);

            var idVotacion = nuevaVotacion.Id;

            foreach (var criterioDto in criterios)
            {
                if (criterioDto.IdCategoriaEvento == Guid.Empty)
                    throw new Exception("Cada criterio debe pertenecer a una categoría del evento.");

                if (!await dimensionRepository.CategoriaEventoPerteneceAlEventoAsync(criterioDto.IdCategoriaEvento, Id_evento))
                    throw new Exception("Un criterio está asociado a una categoría que no pertenece al evento.");

                var nuevoCriterio = new DimensionesDeValor
                {
                    Id = criterioDto.Id == Guid.Empty ? Guid.NewGuid() : criterioDto.Id,
                    Id_votacion = idVotacion,
                    Id_categoria_evento = criterioDto.IdCategoriaEvento,
                    Nombre = criterioDto.Nombre,
                    Descripcion = criterioDto.Descripcion,
                    Peso = criterioDto.Peso
                };
                await dimensionRepository.CrearCriterioAsync(nuevoCriterio);
            }

            foreach (var idUsuarioJurado in Jurados)
            {
                var idRolJurado = await eventoUsuarioRolRepository.ObtenerIdRolJuradoAsync();
                if (idRolJurado == Guid.Empty)
                    throw new Exception("No existe el rol Jurado");

                var nuevoJurado = new EventoUsuarioRol
                {
                    EventoId = Id_evento,
                    UsuarioId = idUsuarioJurado,
                    RolId = idRolJurado
                };

                await eventoUsuarioRolRepository.CrearAsync(nuevoJurado);
            }

            if (estadoVotacion == EstadoVotacion.Abierta)
            {
                try { await NotificarAperturaVotacionAsync(Id_evento); }
                catch { /* notificación no crítica */ }
            }
        }

        public async Task NotificarAperturaVotacionAsync(Guid idEvento)
        {
            var usuarios = await usuarioRepository.ObtenerConEmailPorEventoAsync(idEvento);
            if (!usuarios.Any()) return;

            var notificador = new EventoNotificador();
            foreach (var usuario in usuarios)
                notificador.suscribir(new EmailObserver(usuario.Email!, emailSender));

            await notificador.notificarAsync(
                "¡Nueva votación abierta!",
                "Se ha abierto una nueva votación en un evento en el que participas. Entra en Votify para votar.");
        }

        public async Task EditarVotacion(Guid Id, Guid Id_evento, Guid? idCategoriaEvento, DateTime inicioVotacion, DateTime finVotacion, bool permiteAutoVoto, TipoVotacion tipoVotacion, List<CrearCriterioDTO> criterios, int NumProy, bool permiteComentarios, bool comentariosObligatorios, bool comentariosAnonimos, bool verRes,List<Guid> Jurados, EstadoVotacion estadoVotacion)
        {
            var votacionActual = await votacionRepository.ObtenerPorIdAsync(Id);

            if (votacionActual == null)
                throw new Exception("La votación no existe");
            System.Diagnostics.Debug.WriteLine($"Tipo actual real: {votacionActual.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"Tipo actual enum: {votacionActual.Tipo}");
            System.Diagnostics.Debug.WriteLine($"Tipo recibido: {tipoVotacion}");

            if (Id_evento == Guid.Empty)
                throw new Exception("El evento es obligatorio");
            if (idCategoriaEvento.HasValue && idCategoriaEvento.Value != Guid.Empty &&
                !await dimensionRepository.CategoriaEventoPerteneceAlEventoAsync(idCategoriaEvento.Value, Id_evento))
                throw new Exception("La categoría de la votación no pertenece al evento");

            if (finVotacion < inicioVotacion)
                throw new Exception("La fecha de fin no puede ser anterior a la fecha de inicio");
            (comentariosObligatorios, comentariosAnonimos) = NormalizarOpcionesComentarios(
                permiteComentarios, comentariosObligatorios, comentariosAnonimos);
            if (votacionActual.Tipo == tipoVotacion)
            {
                votacionActual.Id_evento = Id_evento;
                votacionActual.Id_categoria_evento = idCategoriaEvento == Guid.Empty ? null : idCategoriaEvento;
                votacionActual.FechaInicio = inicioVotacion.ToUniversalTime();
                votacionActual.FechaFin = finVotacion.ToUniversalTime();
                votacionActual.PermiteAutoVoto = permiteAutoVoto;
                votacionActual.NumProy = NumProy;
                votacionActual.PermiteComentarios = permiteComentarios;
                votacionActual.ComentariosObligatorios = comentariosObligatorios;
                votacionActual.ComentariosAnonimos = comentariosAnonimos;
                votacionActual.EstadoVotacion = estadoVotacion;
                votacionActual.VerRes = verRes;

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
                    comentariosObligatorios,
                    comentariosAnonimos,
                    verRes,
                    estadoVotacion
                );

                nuevaVotacion.Id = votacionActual.Id;
                nuevaVotacion.Id_categoria_evento = idCategoriaEvento == Guid.Empty ? null : idCategoriaEvento;

                await votacionRepository.ReemplazarVotacionAsync(votacionActual, nuevaVotacion);
            }

            var criteriosExistentes = await dimensionRepository.ObtenerCriterioPorVotacionAsync(Id);

            foreach (var criterioDto in criterios)
            {
                if (criterioDto.IdCategoriaEvento == Guid.Empty)
                    throw new Exception("Cada criterio debe pertenecer a una categoría del evento.");

                if (!await dimensionRepository.CategoriaEventoPerteneceAlEventoAsync(criterioDto.IdCategoriaEvento, Id_evento))
                    throw new Exception("Un criterio está asociado a una categoría que no pertenece al evento.");

                var existente = criteriosExistentes.FirstOrDefault(c => c.Id == criterioDto.Id);

                if (existente == null)
                {
                    var nuevoCriterio = new DimensionesDeValor
                    {
                        Id = criterioDto.Id == Guid.Empty ? Guid.NewGuid() : criterioDto.Id,
                        Id_votacion = Id,
                        Id_categoria_evento = criterioDto.IdCategoriaEvento,
                        Nombre = criterioDto.Nombre,
                        Descripcion = criterioDto.Descripcion,
                        Peso = criterioDto.Peso
                    };

                    await dimensionRepository.CrearCriterioAsync(nuevoCriterio);
                }
                else
                {
                    existente.Nombre = criterioDto.Nombre;
                    existente.Descripcion = criterioDto.Descripcion;
                    existente.Peso = criterioDto.Peso;
                    existente.Id_categoria_evento = criterioDto.IdCategoriaEvento;
                    await dimensionRepository.EditarCriterioAsync(existente);
                }
            }

            var criteriosABorrar = criteriosExistentes
                .Where(c => !criterios.Any(dto => dto.Id == c.Id))
                .ToList();

            foreach (var criterio in criteriosABorrar)
            {
                if (await dimensionRepository.TieneVotosAsociadosAsync(criterio.Id))
                    throw new Exception($"No se puede eliminar el criterio '{criterio.Nombre}' porque ya tiene votos asociados.");

                await dimensionRepository.EliminarCriterioAsync(criterio);
            }

            var juradosExistentes = await eventoUsuarioRolRepository.ObtenerJuradosPorEventoAsync(Id_evento);

            foreach (var idUsuarioJurado in Jurados)
            {
                var existente = juradosExistentes
                    .FirstOrDefault(j => j.UsuarioId == idUsuarioJurado);

                var idRolJurado = await eventoUsuarioRolRepository.ObtenerIdRolJuradoAsync();
                if (idRolJurado == Guid.Empty)
                    throw new Exception("No existe el rol Jurado");

                if (existente == null)
                {
                    var nuevoJurado = new EventoUsuarioRol
                    {
                        EventoId = Id_evento,
                        UsuarioId = idUsuarioJurado,
                        RolId = idRolJurado
                    };

                    await eventoUsuarioRolRepository.CrearAsync(nuevoJurado);
                }
            }

            var juradosABorrar = juradosExistentes
                .Where(j => !Jurados.Any(id => id == j.UsuarioId))
                .ToList();

            foreach (var jurado in juradosABorrar)
            {
                await eventoUsuarioRolRepository.EliminarAsync(jurado);
            }

            if (estadoVotacion == EstadoVotacion.Abierta)
            {
                try { await NotificarAperturaVotacionAsync(Id_evento); }
                catch { /* notificación no crítica */ }
            }
        }
        private static (bool obligatorios, bool anonimos) NormalizarOpcionesComentarios(bool permiteComentarios, bool comentariosObligatorios, bool comentariosAnonimos)
        {
            if (!permiteComentarios)
                return (false, false);

            return (comentariosObligatorios, comentariosAnonimos);
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

        public async Task<List<Votacion>> ObtenerVotacionesEventos(List<Guid> idsEventos)
        {
            return await votacionRepository.ObtenerVotacionesEventos(idsEventos);
        }

        public async Task<List<Voto>> ObtenerVotosVotacion(Guid idVotacion)
        {
            return await votacionRepository.ObtenerVotosVotacion(idVotacion);
        }

        public async Task AbrirVotacionesProgramadasAsync()
        {
            var vencidas = await votacionRepository.ObtenerCerradasVencidasAsync();
            if (!vencidas.Any()) return;

            var eventosANotificar = new List<Guid>();
            foreach (var votacion in vencidas)
            {
                await votacionRepository.AbrirVotacionAsync(votacion.Id);
                eventosANotificar.Add(votacion.Id_evento);
            }

            foreach (var idEvento in eventosANotificar.Distinct())
            {
                try { await NotificarAperturaVotacionAsync(idEvento); }
                catch { /* notificación no crítica */ }
            }
        }
    }
}

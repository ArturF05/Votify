using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Votify.API.DTOs;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using static Votify.API.Components.Pages.AltaProyecto;
namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEvento eventoService;
        private readonly IVotacion votacionService;
        private readonly AppDbContext context;
        private readonly IAzureStorage azureStorage;

        public EventosController(IEvento eventoService, IVotacion votacionService, AppDbContext context, IAzureStorage azureStorage)
        {
            this.eventoService = eventoService;
            this.votacionService = votacionService;
            this.context = context;
            this.azureStorage = azureStorage;
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvento([FromBody] CrearEventoDTOs dto)
        {
            using var transaccion = await context.Database.BeginTransactionAsync();
            string? imagenUrl = null;

            if (dto.ImagenBytes != null && !string.IsNullOrEmpty(dto.ImagenNombre))
            {
                using var ms = new MemoryStream(dto.ImagenBytes);

                imagenUrl = await azureStorage.SubirArchivoAsync(ms, dto.ImagenNombre);
            }
            try
            {
                var evento = await eventoService.CrearEvento(
                    dto.Nombre,
                    dto.Id_categoria,
                    dto.FechaInicio,
                    dto.Descripcion,
                    dto.FechaFin,
                    dto.Tipo,
                    dto.UsuarioCreadorId,
                    imagenUrl
                );
                await context.SaveChangesAsync();
                foreach (var categoriaDto in dto.CategoriasEvento)
                {
                    if (string.IsNullOrWhiteSpace(categoriaDto.Nombre))
                        continue;

                    var categoriaEvento = new CategoriaEvento
                    {
                        Id = categoriaDto.Id == Guid.Empty ? Guid.NewGuid() : categoriaDto.Id,
                        Id_evento = evento.Id,
                        Nombre = categoriaDto.Nombre.Trim()
                    };

                    await context.CategoriaEvento.AddAsync(categoriaEvento);

                }
                await context.SaveChangesAsync();

                var jurados = new List<Guid>();
                for (int i = 0; i < dto.dTOsVotacion.Count; i++)
                {
                        await votacionService.CrearVotacion(evento.Id, dto.dTOsVotacion[i].Id_categoria_evento, dto.dTOsVotacion[i].FechaInicio, dto.dTOsVotacion[i].FechaFin, dto.dTOsVotacion[i].PermiteAutoVoto, dto.dTOsVotacion[i].TipoVotacion, dto.dTOsVotacion[i].Num_Proy, dto.dTOsVotacion[i].PermiteComentarios, dto.dTOsVotacion[i].ComentariosObligatorios, dto.dTOsVotacion[i].VerRes, jurados, dto.dTOsVotacion[i].EstadoVotacion, dto.dTOsVotacion[i].Criterios);
                        await context.SaveChangesAsync();
                }
                await context.SaveChangesAsync();
                await transaccion.CommitAsync();

                return Ok(new
                {
                    evento.Id,
                    evento.Nombre
                });
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                return BadRequest(new
                {
                    mensaje = ex.Message,
                    inner = ex.InnerException?.Message,
                    inner2 = ex.InnerException?.InnerException?.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarEvento(Guid id, [FromBody] CrearEventoDTOs dto)
        {
            string? imagenUrl = null;

            if (dto.ImagenBytes != null && !string.IsNullOrEmpty(dto.ImagenNombre))
            {
                using var ms = new MemoryStream(dto.ImagenBytes);

                imagenUrl = await azureStorage.SubirArchivoAsync(ms, dto.ImagenNombre);
            }
            try
            {
                await eventoService.EditarEvento(
                    id,
                    dto.Nombre,
                    dto.Id_categoria,
                    dto.FechaInicio,
                    dto.Descripcion,
                    dto.FechaFin,
                    imagenUrl
                );

                return Ok(new { mensaje = "Evento editado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEvento(Guid id)
        {
            try
            {
                await eventoService.EliminarEvento(id);
                return Ok(new { mensaje = "Evento eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPatch("{id}/archivo")]
        public async Task<IActionResult> ArchivarEvento(Guid id, [FromQuery] bool archivar)
        {
            try
            {
                await eventoService.Archivo(id, archivar);

                if (archivar)
                    return Ok(new { mensaje = "Evento archivado correctamente" });
                else
                    return Ok(new { mensaje = "Evento desarchivado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarEvento([FromQuery] string? nombre, [FromQuery] Guid idCategoria, [FromQuery] Guid idUsuario)
        {
            try
            {
                nombre ??= "";
                var categoria = idCategoria;
                return Ok(await eventoService.BuscarEvento(nombre, idCategoria, idUsuario));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerEventos()
        {
            try
            {
                var eventos = await eventoService.ObtenerTodos();

                var resultado = eventos.Select(e => new EventoListadoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    ImagenUrl = e.ImagenUrl
                }).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpGet("numVotos")]
        public async Task<int> NumeroVotosPorEvento([FromQuery] Guid idEvento)
        {
            return await eventoService.NumeroVotosPorEvento(idEvento);
        }
        [HttpGet("organizador")]
        public async Task<List<Votify.Core.Entities.Evento>> ObtenerEventosOrganizador(Guid idUsuario)
        {
            return await eventoService.ObtenerEventosOrganizador(idUsuario);
        }

        [HttpPatch("{id}/apuntarse")]
        public async Task<IActionResult> ApuntarseAEvento(Guid id, [FromQuery] Guid usuarioId, [FromQuery] bool apuntar)
        {
            try
            {
                await eventoService.ApuntarseAsync(id, usuarioId, apuntar);

                if (apuntar)
                    return Ok(new { mensaje = "Usuario apuntado correctamente al evento" });
                else
                    return Ok(new { mensaje = "Usuario desapuntado correctamente del evento" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> ObtenerDashboardEvento(Guid id)
        {
            try
            {
                return Ok(await eventoService.ObtenerDashboardEventoAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            } 
        }
        [HttpGet("estadisticas-organizador")]
        public async Task<DashboardOrganizadorStatsDTO> EstadisticasOrganizador([FromQuery] Guid idUsuario)
        {
            var eventosIds = await context.EventoUsuarioRol
                .Where(eur => eur.UsuarioId == idUsuario && eur.Rol.Nombre == "Organizador")
                .Select(eur => eur.EventoId)
                .Distinct()
                .ToListAsync();

            var eventosActivos = await context.Evento
                .CountAsync(e => eventosIds.Contains(e.Id) && !e.Archivado);

            var proyectosTotales = await context.Proyecto
                .CountAsync(p => eventosIds.Contains(p.Id_evento));

            var participantes = await context.EventoUsuarioRol
                .Where(eur => eventosIds.Contains(eur.EventoId) && eur.Rol.Nombre == "Participante")
                .Select(eur => eur.UsuarioId)
                .Distinct()
                .CountAsync();

            var votacionesIds = await context.Votacion
                .Where(v => eventosIds.Contains(v.Id_evento))
                .Select(v => v.Id)
                .ToListAsync();

            var votosEmitidos = await context.Voto
                .CountAsync(v => votacionesIds.Contains(v.Id_votacion));

            return new DashboardOrganizadorStatsDTO
            {
                EventosActivos = eventosActivos,
                ProyectosTotales = proyectosTotales,
                Participantes = participantes,
                VotosEmitidos = votosEmitidos
            };
        }
        [HttpGet("numProyectos")]
        public async Task<int> NumProyectosPorEvento([FromQuery] Guid idEvento)
        {
            return await context.Proyecto.CountAsync(p => p.Id_evento == idEvento);
        }

        [HttpGet("{id}/detalle")]
        public async Task<IActionResult> ObtenerDetalleEvento(Guid id)
        {
            var evento = await eventoService.ObtenerEventoDetalleAsync(id);

            if (evento == null)
                return NotFound(new { mensaje = "Evento no encontrado" });

            return Ok(evento);
        }
    }
}

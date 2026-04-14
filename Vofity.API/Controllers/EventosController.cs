using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Votify.API.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEvento eventoService;
        private readonly IVotacion votacionService;
        private readonly AppDbContext context;

        public EventosController(IEvento eventoService, IVotacion votacionService, AppDbContext context)
        {
            this.eventoService = eventoService;
            this.votacionService = votacionService;
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvento([FromBody] CrearEventoDTOs dto)
        {
            using var transaccion = await context.Database.BeginTransactionAsync();

            try
            {
                var evento = await eventoService.CrearEvento(
                    dto.Nombre,
                    dto.Id_categoria,
                    dto.FechaInicio,
                    dto.Descripcion,
                    dto.FechaFin,
                    dto.Tipo,
                    dto.UsuarioCreadorId
                );
                await context.SaveChangesAsync();
                for (int i = 0; i < dto.dTOsVotacion.Count; i++)
                    {
                        await votacionService.CrearVotacion(evento.Id, dto.dTOsVotacion[i].FechaInicio, dto.dTOsVotacion[i].FechaFin, dto.dTOsVotacion[i].PermiteAutoVoto, dto.dTOsVotacion[i].TipoVotacion);
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
            try
            {
                await eventoService.EditarEvento(
                    id,
                    dto.Nombre,
                    dto.Id_categoria,
                    dto.FechaInicio,
                    dto.Descripcion,
                    dto.FechaFin
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
        public async Task<IActionResult> BuscarEvento([FromQuery] string nombre, [FromQuery] Guid idCategoria)
        {
            try
            {
                nombre ??= "";
                var categoria = idCategoria;
                var eventos = await eventoService.BuscarEvento(nombre, idCategoria);
                return Ok(eventos);
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
                    Nombre = e.Nombre
                }).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
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
                var resultado = await eventoService.ObtenerDashboardEventoAsync(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

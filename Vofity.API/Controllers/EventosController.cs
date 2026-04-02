using Microsoft.AspNetCore.Mvc;
using Votify.API.DTOs;
using Votify.Core.Interfaces;
namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEvento eventoService;

        public EventosController(IEvento eventoService)
        {
            this.eventoService = eventoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearEvento([FromBody] CrearEventoDTOs dto)
        {
            try
            {
                await eventoService.CrearEvento(
                    dto.Nombre,
                    dto.Id_categoria,
                    dto.FechaInicio,
                    dto.Descripcion,
                    dto.FechaFin,
                    dto.Tipo
                );

                return Ok(new { mensaje = "Evento creado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
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
                var categoria = idCategoria;
                var eventos = await eventoService.BuscarEvento(nombre, idCategoria);
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Votify.API.DTOs;
using Votify.Core.DTOs;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Votify.Infrastructure.Data;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/v1/votaciones")]
    public class VotacionController : ControllerBase
    {
        private readonly IVotacion votacionService;
        private readonly AppDbContext context;


        public VotacionController(IVotacion votacionService, AppDbContext context)
        {
            this.votacionService = votacionService;
            this.context = context;
        }

        [HttpPost("{idVotacion}/votos")]
        public async Task<IActionResult> EmitirVoto(Guid idVotacion, [FromBody] EmitirVotoDTO dto)
        {
            try
            {
                var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

                if (!resultado)
                    return BadRequest("No se pudo registrar el voto.");

                return Ok(new { mensaje = "Voto registrado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message,
                    inner = ex.InnerException?.Message,
                    inner2 = ex.InnerException?.InnerException?.Message
                });
            }
        }
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarVotacion([FromQuery] Guid id_evento, [FromQuery] int tipoVotacion, [FromQuery] int permiteAutoVoto, [FromQuery] bool verFinalizadas)
        {
            try
            {
                var votaciones = await votacionService.BuscarVotacion(id_evento, tipoVotacion, permiteAutoVoto, verFinalizadas);
                return Ok(votaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CrearVotacion([FromBody] CrearVotacionDTOs dto)
        {
            using var transaccion = await context.Database.BeginTransactionAsync();
            try
            {
                await votacionService.CrearVotacion(
                    dto.Id_evento,
                    dto.FechaInicio,
                    dto.FechaFin,
                    dto.PermiteAutoVoto,
                    dto.TipoVotacion,
                    dto.Num_Proy
                );
                await context.SaveChangesAsync();
                await transaccion.CommitAsync();
                return Ok(new { mensaje = "Votación creada correctamente" });
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
        public async Task<IActionResult> EditarVotacion(Guid id, [FromBody] CrearVotacionDTOs dto)
        {
            try
            {
                await votacionService.EditarVotacion(
                    id,
                    dto.Id_evento,
                    dto.FechaInicio,
                    dto.FechaFin,
                    dto.PermiteAutoVoto,
                    dto.TipoVotacion,
                    dto.Criterios
                );

                return Ok(new { mensaje = "Evento editado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarVotacion(Guid id)
        {
            try
            {
                await votacionService.EliminarVotacion(id);
                return Ok(new { mensaje = "Votacion eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

    }

}

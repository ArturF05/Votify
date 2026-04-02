using Microsoft.AspNetCore.Mvc;
using Votify.Core.DTOs;
using Votify.Core.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/v1/votaciones")]
    public class VotacionController : ControllerBase
    {
        private readonly IVotacion votacionService;

        public VotacionController(IVotacion votacionService)
        {
            this.votacionService = votacionService;
        }

        [HttpPost("{idVotacion}/votos")]
        public async Task<IActionResult> EmitirVoto(Guid idVotacion, [FromBody] EmitirVotoDTO dto)
        {
            var resultado = await votacionService.RegistrarVotoAsync(idVotacion, dto);

            if(!resultado) { return BadRequest("No se pudo registrar el voto."); }

            return Ok("Voto registrado correctamente.");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Votify.Core.DTOs;
using Votify.Core.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VotacionController : ControllerBase
    {
        private readonly IVotacion votacionService;

        public VotacionController(IVotacion votacionService)
        {
            this.votacionService = votacionService;
        }

        [HttpPost("emitir")]
        public async Task<IActionResult> EmitirVoto([FromBody] EmitirVotoDTO dto)
        {
            var resultado = await votacionService.RegistrarVotoAsync(dto);

            if(!resultado) { return BadRequest("No se pudo registrar el voto."); }

            return Ok("Voto registrado correctamente.");
        }
    }
}

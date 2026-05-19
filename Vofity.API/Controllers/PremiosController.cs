using Microsoft.AspNetCore.Mvc;
using Votify.API.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/v1/premios")]
    public class PremiosController : ControllerBase
    {
        private readonly IPremio premioService;
        private readonly IAzureStorage azureStorage;

        public PremiosController(IPremio premioService, IAzureStorage azureStorage)
        {
            this.premioService = premioService;
            this.azureStorage = azureStorage;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPremios([FromQuery] Guid? idCategoriaEvento)
        {
            var premios = await premioService.ObtenerPremiosAsync(NormalizarGuid(idCategoriaEvento));
            return Ok(premios.Select(MapPremio));
        }

        [HttpGet("{idPremio:guid}")]
        public async Task<IActionResult> ObtenerPremio(Guid idPremio)
        {
            var premio = await premioService.ObtenerPorIdAsync(idPremio);
            if (premio == null)
                return NotFound(new { mensaje = "El premio no existe." });

            return Ok(MapPremio(premio));
        }

        [HttpPost]
        public async Task<IActionResult> CrearPremio([FromBody] CrearPremioDTO dto)
        {
            try
            {
                var imagenUrl = await SubirImagenAsync(dto.ImagenBytes, dto.ImagenNombre);
                var premio = await premioService.CrearPremioAsync(
                    dto.IdCategoriaEvento,
                    dto.Nombre,
                    dto.Descripcion,
                    dto.Orden,
                    dto.IdsVotaciones,
                    imagenUrl,
                    dto.Puesto);

                return Ok(MapPremio(premio));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{idPremio:guid}")]
        public async Task<IActionResult> EditarPremio(Guid idPremio, [FromBody] EditarPremioDTO dto)
        {
            try
            {
                var premioActual = await premioService.ObtenerPorIdAsync(idPremio);
                if (premioActual == null)
                    return NotFound(new { mensaje = "El premio no existe." });

                var imagenUrl = dto.QuitarImagen ? null : premioActual.ImagenUrl;
                var reemplazaImagen = dto.ImagenBytes != null && !string.IsNullOrWhiteSpace(dto.ImagenNombre);
                if (reemplazaImagen)
                    imagenUrl = await SubirImagenAsync(dto.ImagenBytes, dto.ImagenNombre);

                var premio = await premioService.EditarPremioAsync(
                    idPremio,
                    dto.Nombre,
                    dto.Descripcion,
                    dto.Orden,
                    imagenUrl,
                    dto.Puesto,
                    dto.IdsVotaciones);

                if ((dto.QuitarImagen || reemplazaImagen) && !string.IsNullOrWhiteSpace(premioActual.ImagenUrl))
                    await azureStorage.EliminarArchivoAsync(premioActual.ImagenUrl);

                return Ok(MapPremio(premio));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{idPremio:guid}/votaciones")]
        public async Task<IActionResult> ActualizarVotacionesPremio(Guid idPremio, [FromBody] VincularPremioVotacionesDTO dto)
        {
            try
            {
                var vinculos = await premioService.ActualizarVotacionesPremioAsync(idPremio, dto.IdsVotaciones);
                return Ok(vinculos.Select(MapVinculo));
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("~/api/v1/votaciones/{idVotacion:guid}/premios")]
        public async Task<IActionResult> ObtenerPremiosVotacion(Guid idVotacion, [FromQuery] Guid? idCategoriaEvento)
        {
            var premios = await premioService.ObtenerPremiosPorVotacionAsync(idVotacion, NormalizarGuid(idCategoriaEvento));
            return Ok(premios.Select(MapPremio));
        }

        [HttpDelete("{idPremio:guid}")]
        public async Task<IActionResult> EliminarPremio(Guid idPremio)
        {
            try
            {
                await premioService.EliminarPremioAsync(idPremio);
                return Ok(new { mensaje = "Premio eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        private static Guid? NormalizarGuid(Guid? id)
        {
            return id.HasValue && id.Value != Guid.Empty
                ? id.Value
                : null;
        }

        private static object MapPremio(Premio premio)
        {
            return new
            {
                premio.Id,
                IdCategoriaEvento = premio.Id_categoria_evento,
                Categoria = premio.CategoriaEvento == null
                    ? null
                    : new
                    {
                        premio.CategoriaEvento.Id,
                        premio.CategoriaEvento.Nombre,
                        IdEvento = premio.CategoriaEvento.Id_evento
                    },
                premio.Nombre,
                premio.Descripcion,
                premio.ImagenUrl,
                premio.Orden,
                premio.Puesto,
                Votaciones = premio.PremioVotaciones.Select(MapVinculo)
            };
        }

        private static object MapVinculo(PremioVotacion vinculo)
        {
            return new
            {
                IdPremio = vinculo.Id_premio,
                IdVotacion = vinculo.Id_votacion,
                Votacion = vinculo.Votacion == null
                    ? null
                    : new
                    {
                        vinculo.Votacion.Id,
                        IdEvento = vinculo.Votacion.Id_evento,
                        IdCategoriaEvento = vinculo.Votacion.Id_categoria_evento,
                        Tipo = vinculo.Votacion.Tipo.ToString(),
                        vinculo.Votacion.FechaInicio,
                        vinculo.Votacion.FechaFin,
                        vinculo.Votacion.EstadoVotacion
                    }
            };
        }

        private async Task<string?> SubirImagenAsync(byte[]? imagenBytes, string? imagenNombre)
        {
            if (imagenBytes == null || string.IsNullOrWhiteSpace(imagenNombre))
                return null;

            using var ms = new MemoryStream(imagenBytes);
            return await azureStorage.SubirArchivoAsync(ms, imagenNombre);
        }
    }
}

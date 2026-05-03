using Microsoft.AspNetCore.Mvc;
using Votify.Core.DTOs;
using Votify.Core.Interfaces;
using Votify.Core.Services;

[ApiController]
[Route("api/v1/[controller]")]
public class ProyectoController : ControllerBase  
{
    private readonly IProyectoService _proyectoService;

    public ProyectoController(IProyectoService proyectoService)
    {
        _proyectoService = proyectoService;
    }

    // POST /api/v1/proyectos (crear)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearProyectoDto dto)
    {
        if (dto.IdEvento == null)
            return BadRequest("Debes seleccionar un evento");
        if (string.IsNullOrEmpty(dto.Titulo))
            return BadRequest("El título es obligatorio");
        if (string.IsNullOrWhiteSpace(dto.Descripcion))
            return BadRequest("La descripción es obligatoria");

        var resultado = await _proyectoService.CrearProyecto(dto.Titulo, dto.Descripcion,dto.IdEvento.Value, dto.Id_Creador);

        if (resultado)
            return StatusCode(201, new { status = "success", message = "Proyecto creado" });

        return BadRequest(new { status = "error", message = "No se pudo crear" });
    }

    [HttpGet("{idProyecto}/sintesis-ia")]
    public async Task<ActionResult<SintesisIAResultadoDTO>> GenerarSintesisIA(Guid idProyecto)
    {
        var resultado = await _proyectoService.GenerarSintesisIA(idProyecto);
        return Ok(resultado);
    }
}
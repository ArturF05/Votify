using Microsoft.AspNetCore.Mvc;
using Votify.Core.DTOs;
using Votify.Core.Interfaces;

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

        if (string.IsNullOrEmpty(dto.Titulo))
            return BadRequest(new
            {
                status = "error",
                statusCode = 400,
                message = "Datos inválidos",
                details = new[] { new { field = "titulo", issue = "El título es requerido" } }
            });
        if (dto.IdEvento == null)
            return BadRequest("El evento es obligatorio");

        var resultado = await _proyectoService.CrearProyecto(dto.Titulo, dto.Descripcion,dto.IdEvento.Value);

        if (resultado)
            return StatusCode(201, new { status = "success", message = "Proyecto creado" });

        return BadRequest(new { status = "error", message = "No se pudo crear" });
    }
}
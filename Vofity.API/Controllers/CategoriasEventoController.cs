using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Votify.API.DTOs;
using Votify.Core.Entities;
using Votify.Infrastructure.Data;

namespace Votify.API.Controllers
{
    [ApiController]
    [Route("api/v1/eventos/{idEvento:guid}/categorias")]
    public class CategoriasEventoController : ControllerBase
    {
        private readonly AppDbContext contexto;

        public CategoriasEventoController(AppDbContext contexto)
        {
            this.contexto = contexto;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategoriasEvento(Guid idEvento)
        {
            var categorias = await contexto.CategoriaEvento
                .Where(c => c.Id_evento == idEvento)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre
                })
                .ToListAsync();

            return Ok(categorias);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCategoriaEvento(Guid idEvento, [FromBody] CrearCategoriaEventoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre de la categoría es obligatorio." });

            var existeEvento = await contexto.Evento.AnyAsync(e => e.Id == idEvento);
            if (!existeEvento)
                return NotFound(new { mensaje = "El evento no existe." });

            var categoria = new CategoriaEvento
            {
                Id = Guid.NewGuid(),
                Id_evento = idEvento,
                Nombre = dto.Nombre.Trim()
            };

            await contexto.CategoriaEvento.AddAsync(categoria);
            await contexto.SaveChangesAsync();

            return Ok(new
            {
                categoria.Id,
                categoria.Nombre
            });
        }

        [HttpGet("{idCategoriaEvento:guid}/criterios")]
        public async Task<IActionResult> ObtenerCriterios(Guid idEvento, Guid idCategoriaEvento)
        {
            var categoriaValida = await contexto.CategoriaEvento
                .AnyAsync(c => c.Id == idCategoriaEvento && c.Id_evento == idEvento);

            if (!categoriaValida)
                return NotFound(new { mensaje = "La categoría no pertenece al evento." });

            var criterios = await contexto.DimensionesDeValor
                .Where(c => c.Id_categoria_evento == idCategoriaEvento)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Descripcion,
                    c.Peso
                })
                .ToListAsync();

            return Ok(criterios);
        }

        [HttpPost("{idCategoriaEvento:guid}/criterios")]
        public async Task<IActionResult> CrearCriterio(Guid idEvento, Guid idCategoriaEvento, [FromBody] CrearCriterioCategoriaDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre del criterio es obligatorio." });

            if (dto.Peso <= 0)
                return BadRequest(new { mensaje = "El peso del criterio debe ser mayor que 0." });

            var categoriaValida = await contexto.CategoriaEvento
                .AnyAsync(c => c.Id == idCategoriaEvento && c.Id_evento == idEvento);

            if (!categoriaValida)
                return NotFound(new { mensaje = "La categoría no pertenece al evento." });

            var criterio = new DimensionesDeValor
            {
                Id = Guid.NewGuid(),
                Id_categoria_evento = idCategoriaEvento,
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim() ?? "",
                Peso = dto.Peso
            };

            await contexto.DimensionesDeValor.AddAsync(criterio);
            await contexto.SaveChangesAsync();

            return Ok(new
            {
                criterio.Id,
                criterio.Nombre,
                criterio.Descripcion,
                criterio.Peso
            });
        }
    }
}

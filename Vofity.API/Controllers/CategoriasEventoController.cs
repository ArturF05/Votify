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

        [HttpPut("{idCategoriaEvento:guid}")]
        public async Task<IActionResult> EditarCategoriaEvento(Guid idEvento, Guid idCategoriaEvento, [FromBody] CrearCategoriaEventoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { mensaje = "El nombre de la categoría es obligatorio." });

            var categoria = await contexto.CategoriaEvento
                .FirstOrDefaultAsync(c => c.Id == idCategoriaEvento && c.Id_evento == idEvento);

            if (categoria == null)
                return NotFound(new { mensaje = "La categoría no pertenece al evento." });

            categoria.Nombre = dto.Nombre.Trim();
            await contexto.SaveChangesAsync();

            return Ok(new { categoria.Id, categoria.Nombre });
        }

        [HttpDelete("{idCategoriaEvento:guid}")]
        public async Task<IActionResult> EliminarCategoriaEvento(Guid idEvento, Guid idCategoriaEvento)
        {
            var categoria = await contexto.CategoriaEvento
                .FirstOrDefaultAsync(c => c.Id == idCategoriaEvento && c.Id_evento == idEvento);

            if (categoria == null)
                return NotFound(new { mensaje = "La categoría no pertenece al evento." });

            var tieneVotos = await contexto.Voto.AnyAsync(v => v.Id_categoria == idCategoriaEvento);
            if (tieneVotos)
                return BadRequest(new { mensaje = "No se puede eliminar una categoría con votos asociados." });

            contexto.CategoriaEvento.Remove(categoria);
            await contexto.SaveChangesAsync();

            return Ok(new { mensaje = "Categoría eliminada correctamente." });
        }
    }
}

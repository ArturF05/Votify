using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Votify.Infrastructure.Data;

namespace Vofity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext contexto;

        public CategoriasController(AppDbContext contexto)
        {
            this.contexto = contexto;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            var categorias = await contexto.Categoria.ToListAsync();
            return Ok(categorias);
        }
    }
}
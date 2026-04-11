using Votify.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Votify.Core.Interfaces;

[ApiController]
[Route("api/v1/autoriza")]
public class AutorizaController : ControllerBase
{
    private readonly IUsuarioRepository _repo;

    public AutorizaController(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Login dto)
    {
        var usuario = await _repo.GetByCredencialesAsync(dto.Usuario, dto.Password);

        if (usuario == null)
            return Unauthorized();

        return Ok(usuario);
    }

}
// debugging para verificar que los datos se reciben correctamente 
/* Console.WriteLine($"Intento de login: Usuario='{request.Usuario}', Password='{request.Password}'");

 if (request == null || string.IsNullOrEmpty(request.Usuario))
 {
     return BadRequest("El servidor recibió un objeto vacío");

 }*/
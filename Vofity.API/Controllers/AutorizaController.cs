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
    public async Task<IActionResult> Login([FromBody] Login request)
    {
        // AÑADE ESTO PARA VER QUÉ LLEGA REALMENTE
        Console.WriteLine($"Intento de login: Usuario='{request.Usuario}', Password='{request.Password}'");

        var usuario = await _repo.ObtenerPorCredencialesAsync(request.Usuario, request.Password);

        if (usuario == null)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        return Ok(new { message = "Login correcto" });
    }
}

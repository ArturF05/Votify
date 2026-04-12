using Microsoft.AspNetCore.Mvc;
using Votify.API.DTOs;
using Votify.Core.DTOs;
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
        var usuario = await _repo.ObtenerPorCredencialesAsync(request.Usuario, request.Password);

        if (usuario == null)
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        return Ok(new LoginRespuesta
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol?.Nombre ?? string.Empty
        });
    }
}
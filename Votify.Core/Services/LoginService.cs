using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Interfaces;


namespace Votify.Core.Services
{
    public class LoginService : ILogin
    {
        private readonly HttpClient _http;
        private readonly UsuarioSesion _usuarioSesion;

        public LoginService(HttpClient http, UsuarioSesion usuarioSesion)
        {
            _http = http;
            _usuarioSesion = usuarioSesion;
        }

        public async Task<Usuario?> ValidarUsuarioAsync(string usuario, string password)
        {
            try
            {
                var loginDto = new LoginC
                {
                    Usuario = usuario,
                    Password = password
                };

                var response = await _http.PostAsJsonAsync("api/v1/autoriza/login", loginDto);

                if (!response.IsSuccessStatusCode)
                    return false;

                var respuesta = await response.Content.ReadFromJsonAsync<LoginRespuesta>();
                Console.WriteLine($"Usuario logueado ID: {respuesta.Id}");

                if (respuesta == null)
                    return false;

                _usuarioSesion.IdUsuario = respuesta.Id;
                _usuarioSesion.NombreUsuario = respuesta.Nombre;
                _usuarioSesion.Rol = respuesta.Rol;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
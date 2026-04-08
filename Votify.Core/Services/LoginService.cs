using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class LoginService : ILogin
    {
        private readonly HttpClient _http;
        private readonly IUsuarioSesion _sesion; 

        public LoginService(HttpClient http, IUsuarioSesion sesion)
        {
            _http = http;
            _sesion = sesion;
        }

        public async Task<Usuario?> ValidarUsuarioAsync(string usuario, string password)
        {
            try
            {
                var loginDto = new { Usuario = usuario, Password = password };
                var response = await _http.PostAsJsonAsync("api/v1/autoriza/login", loginDto);

                if (!response.IsSuccessStatusCode) { return null; }

                var usuarioRespuesta = await response.Content.ReadFromJsonAsync<Usuario>();

                if (usuarioRespuesta != null)
                {
                    _sesion.UsuarioActual = usuarioRespuesta;
                }

                return usuarioRespuesta;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

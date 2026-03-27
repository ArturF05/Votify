using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class LoginService : ILogin
    {
        private readonly HttpClient _http;

        public LoginService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> ValidarUsuarioAsync(string usuario, string password)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/autoriza/login", new
            {
                usuario,
                password
            });

            return response.IsSuccessStatusCode;
        }
    }
}

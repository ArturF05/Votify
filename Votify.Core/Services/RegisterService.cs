using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.Entities;
using Votify.Core.Interfaces;

namespace Votify.Core.Services
{
    public class RegisterService : IRegister
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public RegisterService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> RegistrarUsuarioAsync(string usuario, string password)
        {
            usuario = usuario?.Trim() ?? "";
            password = password?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                return false;

            var existente = await _usuarioRepository.ObtenerPorNombreAsync(usuario);

            if (existente != null)
                return false;

            var nuevoUsuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = usuario,
                Password = password
            };

            await _usuarioRepository.CrearUsuarioAsync(nuevoUsuario);
            return true;
        }
    }
}
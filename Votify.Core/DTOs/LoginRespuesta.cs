using System;

namespace Votify.Core.DTOs
{
    public class LoginRespuesta
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }
}
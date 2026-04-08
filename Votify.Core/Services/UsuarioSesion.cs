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

namespace Votify.Core.Services;

public class UsuarioSesion : IUsuarioSesion
{
    public Usuario? UsuarioActual { get; set; }
}
// para el futuro, se puede implementar un servicio de autenticación que maneje la sesión del usuario,
// y este servicio podría ser inyectado en los controladores o servicios que lo necesiten.

// De esta manera, se podría mantener la información del usuario actual de manera más segura y estructurada.
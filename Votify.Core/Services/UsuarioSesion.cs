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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Votify.Core.DTOs;
using Votify.Core.Entities;
using Votify.Core.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Votify.Core.Interfaces
{
    public interface IProyectoService
    {
        Task<bool> CrearProyecto(
            string nombre,
            string descripcion,
            Guid IdEvento,
            Guid idCategoriaEvento,
            Guid idCreador
        );
        Task<SintesisIAResultadoDTO> GenerarSintesisIA(Guid idProyecto);
    }
}

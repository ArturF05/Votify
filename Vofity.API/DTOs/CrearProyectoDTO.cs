using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Votify.API.Components.Pages;

namespace Votify.Core.DTOs
{
    public class CrearProyectoDto
    {
        [Required(ErrorMessage = "El título es obligatorio")] 
        [MaxLength(50, ErrorMessage = "El título no puede superar 50 caracteres")]

        [JsonPropertyName("titulo")] // Para mapear "titulo" del JSON al campo Titulo si no coincide el nombre
        public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public Guid? IdEvento { get; set; }
    }
}

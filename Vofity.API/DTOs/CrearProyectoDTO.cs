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

        private const int LONGITUD_MAXIMA_TITULO = 50;
        [MaxLength(LONGITUD_MAXIMA_TITULO, ErrorMessage = "El título no puede superar 50 caracteres")]

        [JsonPropertyName("titulo")] 
        public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public Guid? IdEvento { get; set; }
        public Guid? IdCategoriaEvento { get; set; }
        public Guid Id_Creador { get; set; }
    }
}

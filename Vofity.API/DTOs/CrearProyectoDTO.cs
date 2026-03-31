using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.DTOs
{
    public class CrearProyectoDto
    {
        [Required(ErrorMessage = "El título es obligatorio")] //revisar !!!!!!!!!!!!!!!!
        [MaxLength(50, ErrorMessage = "El título no puede superar 50 caracteres")]
        public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
    }
}

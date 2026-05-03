using System;

namespace Votify.Core.DTOs
{
    public class MaterialDto
    {
        public Guid? Id { get; set; } 
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Url { get; set; }
        public DateTime FechaSubida { get; set; }

        // Propiedades temporales para subir a Azure desde el Service
        public byte[]? ArchivoBytes { get; set; }
        public string? NombreArchivo { get; set; }
        public string? Valor { get; set; } // Guarda la URL temporal o el texto del enlace

        public bool EsNuevo => !Id.HasValue || Id == Guid.Empty;
    }
}
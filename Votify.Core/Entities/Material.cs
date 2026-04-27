using System;

namespace Votify.Core.Entities;

public class Material
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Imagen";
    public string Url { get; set; } = string.Empty;

    public Guid ProyectoId { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    public Proyecto Proyecto { get; set; } = null!;
}
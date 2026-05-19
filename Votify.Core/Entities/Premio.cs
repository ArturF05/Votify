namespace Votify.Core.Entities;

public class Premio
{
    public Guid Id { get; set; }
    public Guid Id_categoria_evento { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ImagenUrl { get; set; }
    public int Orden { get; set; }
    public int Puesto { get; set; } = 1;
    public CategoriaEvento? CategoriaEvento { get; set; }
    public List<PremioVotacion> PremioVotaciones { get; set; } = new();
}

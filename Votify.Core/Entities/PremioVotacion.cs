namespace Votify.Core.Entities;

public class PremioVotacion
{
    public Guid Id_premio { get; set; }
    public Premio Premio { get; set; } = null!;
    public Guid Id_votacion { get; set; }
    public Votacion Votacion { get; set; } = null!;
}

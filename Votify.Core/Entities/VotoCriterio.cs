using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Core.Entities;

public class VotoCriterio
{
    public Guid Id { get; set; }

    [Column("Id_voto")]
    public Guid Id_voto { get; set; }

    [Column("Id_criterio")]
    public Guid Id_criterio { get; set; }

    public int Nota { get; set; }
    public string? Comentario { get; set; }

    [ForeignKey(nameof(Id_voto))]
    public Voto Voto { get; set; }

    [ForeignKey(nameof(Id_criterio))]
    public DimensionesDeValor Criterio { get; set; }
}
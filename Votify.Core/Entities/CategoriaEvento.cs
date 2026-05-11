using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Votify.Core.Entities;

public class CategoriaEvento
{
    [Key]
    public Guid Id { get; set; }

    [Column("Id_evento")]
    public Guid Id_evento { get; set; }

    public string Nombre { get; set; }

    [ForeignKey(nameof(Id_evento))]
    public Evento Evento { get; set; }

    public List<DimensionesDeValor> Criterios { get; set; } = new();
}

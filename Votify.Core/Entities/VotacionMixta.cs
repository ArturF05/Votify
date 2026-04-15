using System.ComponentModel.DataAnnotations.Schema;
using Votify.Core.Entities;

public class VotacionMixta : Votacion
{
    public override TipoVotacion Tipo => TipoVotacion.Mixto;

    [NotMapped]
    public decimal PesoJurado { get; set; } = 0.7m;

    [NotMapped]
    public decimal PesoPublico { get; set; } = 0.3m;
}
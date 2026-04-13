using System.ComponentModel.DataAnnotations.Schema;
using Votify.Core.Entities;

namespace Votify.API.DTOs
{
    public class CrearVotacionDTOs
    {
        public Boolean PermiteAutoVoto { get; set; }
        public Guid Id_evento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TipoVotacion TipoVotacion { get; set; }
    }
}

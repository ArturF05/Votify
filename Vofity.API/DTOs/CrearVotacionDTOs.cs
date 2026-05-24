using System.ComponentModel.DataAnnotations.Schema;
using Votify.Core.Entities;

namespace Votify.API.DTOs
{
    public class CrearVotacionDTOs
    {
        public bool PermiteAutoVoto { get; set; }
        public Guid Id_evento { get; set; }
        public Guid? Id_categoria_evento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TipoVotacion TipoVotacion { get; set; }
        public List<Votify.Core.DTOs.CrearCriterioDTO> Criterios { get; set; } = new();
        public List<Guid> Jurados { get; set; } = new();
        public int Num_Proy { get; set; }
        public bool VerRes {  get; set; }
        public EstadoVotacion EstadoVotacion { get; set; }
        public bool PermiteComentarios { get; set; }
        public bool ComentariosObligatorios { get; set; }
        public bool ComentariosAnonimos { get; set; }
    }
}

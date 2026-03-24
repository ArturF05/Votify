using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votify.Core.Entities;

public class Evento
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateOnly FechaInicio {  get; set; }
    public DateOnly FechaFin {  get; set; }
    [ForeignKey(nameof(Categoria))]
    public Guid Id_categoria { get; set; }
    [Required]
    public Categoria Categoria { get; set; }
    public Rol EventoUsuarioRols { get; set; }
    public Proyecto Proyecto { get; set; }
    public Votacion Votacion { get; set; }
    public bool Archivado {  get; set; }
}

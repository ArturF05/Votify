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
    public DateTime FechaInicio {  get; set; }
    public DateTime FechaFin {  get; set; }
    [ForeignKey(nameof(Categoria))]
    public Guid Id_categoria { get; set; }
    [Required]
    public Categoria Categoria { get; set; }
   
    public ICollection<Proyecto> Proyectos { get; set; }
    public List<Votacion> Votaciones { get; set; }
    public bool Archivado {  get; set; }

    public ICollection<EventoUsuarioRol> EventoUsuarioRoles { get; set; }
    public string? ImagenUrl { get; set; }
}

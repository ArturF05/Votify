using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Votify.Core.Entities;

namespace Votify.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<Voto> Voto { get; set; }
    public DbSet<Votacion> Votacione { get; set; }
    public DbSet<Rol> Role { get; set; }
    public DbSet<Resultado> Resultado { get; set; }
    public DbSet<Proyecto> Proyecto { get; set; }
    public DbSet<Evento> Evento { get; set; }
    public DbSet<Categoria> Categoria { get; set; }
    public DbSet<DimensionesDeValor> DimensionesDeValor { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) //Si no se pone da problemas raros
    {
        
        // Ignorar propiedades sin FK en la BD
        modelBuilder.Entity<Evento>()
            .Ignore(e => e.EventoUsuarioRols);

        modelBuilder.Entity<Evento>()
            .Ignore(e => e.Proyecto);

        modelBuilder.Entity<Evento>()
            .Ignore(e => e.Votacion);
    }
}
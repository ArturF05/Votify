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
    public DbSet<Votacion> Votacion { get; set; }
    public DbSet<Rol> Role { get; set; }
    public DbSet<Resultado> Resultado { get; set; }
    public DbSet<Proyecto> Proyecto { get; set; }
    public DbSet<Evento> Evento { get; set; }
    public DbSet<Categoria> Categoria { get; set; }
    public DbSet<DimensionesDeValor> DimensionesDeValor { get; set; }
    public DbSet<EventoUsuarioRol> EventoUsuarioRol { get; set; }
      
    protected override void OnModelCreating(ModelBuilder modelBuilder) //Si no se pone da problemas raros
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventoHackathon>().HasBaseType<Evento>();
        modelBuilder.Entity<EventoConcurso>().HasBaseType<Evento>();

        // Ignorar propiedades sin FK en la BD
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Evento>()
            .Ignore(e => e.Votacion);

    }
}
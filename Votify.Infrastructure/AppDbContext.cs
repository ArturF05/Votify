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
    public DbSet<VotoCriterio> VotoCriterio { get; set; }
    public DbSet<EventoUsuarioRol> EventoUsuarioRol { get; set; }
    public DbSet<Material> Material { get; set; }
    public DbSet<ProyectoParticipantes> ProyectoParticipantes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventoHackathon>().HasBaseType<Evento>();
        modelBuilder.Entity<EventoConcurso>().HasBaseType<Evento>();

        modelBuilder.Entity<EventoUsuarioRol>(entity =>
        {
            entity.ToTable("EventoUsuarioRol");
            entity.Ignore(e => e.Id);
            entity.HasKey(e => new { e.EventoId, e.UsuarioId, e.RolId });
        });

        modelBuilder.Entity<Votacion>(entity =>
        {
            entity.ToTable("Votacion");

            entity.HasKey(v => v.Id);

            entity.Property(v => v.Id_evento)
                .HasColumnName("Id_evento");

            entity.Property(v => v.FechaInicio)
                .HasColumnName("FechaInicio");

            entity.Property(v => v.FechaFin)
                .HasColumnName("FechaFin");

            entity.Ignore(v => v.Tipo);

            entity.HasOne<Evento>()
                .WithMany(e => e.Votaciones)
                .HasForeignKey(v => v.Id_evento)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(v => v.NumProy)
                .HasColumnName("NumProy");
            entity.Property(v => v.EstadoVotacion).HasColumnName("EstadoVotacion");
            entity.Property(v=> v.VerRes).HasColumnName("VerRes");
        });

        modelBuilder.Entity<VotacionJurado>().HasBaseType<Votacion>();
        modelBuilder.Entity<VotacionPublico>().HasBaseType<Votacion>();
        modelBuilder.Entity<VotacionMixta>().HasBaseType<Votacion>();

        modelBuilder.Entity<Votacion>()
            .HasDiscriminator<int>("TipoVotacion")
            .HasValue<VotacionPublico>(0)
            .HasValue<VotacionJurado>(1)
            .HasValue<VotacionMixta>(2);
        modelBuilder.Entity<DimensionesDeValor>()
            .HasOne(d => d.Votacion)
            .WithMany(v => v.DimensionesDeValores)
            .HasForeignKey(d => d.Id_votacion);

        modelBuilder.Entity<VotoCriterio>(entity =>
        {
            entity.ToTable("VotoCriterio");

            entity.HasKey(vc => vc.Id);

            entity.HasIndex(vc => new { vc.Id_voto, vc.Id_criterio })
                .IsUnique();

            entity.HasOne(vc => vc.Voto)
                .WithMany(v => v.VotosCriterio)
                .HasForeignKey(vc => vc.Id_voto)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(vc => vc.Criterio)
                .WithMany()
                .HasForeignKey(vc => vc.Id_criterio)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProyectoParticipantes>(entity =>
        {
            entity.ToTable("ProyectoParticipantes");

            entity.HasKey(pp => new { pp.Id_proyecto, pp.Id_usuario });

            entity.Property(pp => pp.Id_proyecto)
                .HasColumnName("Id_proyecto");

            entity.Property(pp => pp.Id_usuario)
                .HasColumnName("Id_usuario");

            entity.HasOne(pp => pp.Usuario)
                .WithMany(u => u.ProyectoParticipantes)
                .HasForeignKey(pp => pp.Id_usuario);

            entity.HasOne(pp => pp.Proyecto)
                .WithMany(p => p.ProyectoParticipantes)
                .HasForeignKey(pp => pp.Id_proyecto);
        });
    }
}

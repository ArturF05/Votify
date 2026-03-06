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

    // Aquí se registran las tablas de la base de datos
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Voto> Votos { get; set; }
    public DbSet<Votacion> Votaciones { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Resultado> Resultados { get; set; }
    public DbSet<Proycto> Proyecctos { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<DimensionesDeValor> DimensionesDeValor { get; set; }

}

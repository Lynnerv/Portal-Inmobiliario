using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portal_Inmobiliario.Models;

namespace Portal_Inmobiliario.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Inmueble> Inmuebles => Set<Inmueble>();
        public DbSet<Visita> Visitas => Set<Visita>();
        public DbSet<Reserva> Reservas => Set<Reserva>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<Inmueble>().HasIndex(i => i.Codigo).IsUnique();
            b.Entity<Inmueble>().ToTable(t => t.HasCheckConstraint("CK_Inmueble_Precio_Pos", "Precio > 0"));
            b.Entity<Inmueble>().ToTable(t => t.HasCheckConstraint("CK_Inmueble_M2_Pos", "MetrosCuadrados > 0"));

            b.Entity<Visita>().ToTable(t => t.HasCheckConstraint("CK_Visita_Rango", "FechaInicio < FechaFin"));
            b.Entity<Visita>().HasIndex(v => new { v.InmuebleId, v.FechaInicio, v.FechaFin });

            b.Entity<Reserva>().HasIndex(r => new { r.InmuebleId, r.FechaExpiracion });
        }
    }
}

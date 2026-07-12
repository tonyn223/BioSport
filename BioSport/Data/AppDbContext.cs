using Microsoft.EntityFrameworkCore;
using BioSport.Models;

namespace BioSport.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<RutinaAsignada> RutinasAsignadas { get; set; }
        public DbSet<Progreso> Progresos { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Horario> Horarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relaciones
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany()
                .HasForeignKey(u => u.IdRol);

            modelBuilder.Entity<Membresia>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.IdUsuario);

            modelBuilder.Entity<Membresia>()
                .HasOne(m => m.Plan)
                .WithMany()
                .HasForeignKey(m => m.IdPlan);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.IdUsuario);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Membresia)
                .WithMany()
                .HasForeignKey(p => p.IdMembresia);

            modelBuilder.Entity<Rutina>()
                .HasOne(r => r.Entrenador)
                .WithMany()
                .HasForeignKey(r => r.IdEntrenador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RutinaAsignada>()
                .HasOne(ra => ra.Rutina)
                .WithMany()
                .HasForeignKey(ra => ra.IdRutina);

            modelBuilder.Entity<RutinaAsignada>()
                .HasOne(ra => ra.Cliente)
                .WithMany()
                .HasForeignKey(ra => ra.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Progreso>()
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Progreso>()
                .HasOne(p => p.Entrenador)
                .WithMany()
                .HasForeignKey(p => p.IdEntrenador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asistencia>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.IdUsuario);

            // Precisión de columnas decimales (evita truncamientos silenciosos)
            modelBuilder.Entity<Pago>().Property(p => p.Monto).HasPrecision(10, 2);
            modelBuilder.Entity<Plan>().Property(p => p.Precio).HasPrecision(10, 2);
            modelBuilder.Entity<Progreso>().Property(p => p.Peso).HasPrecision(5, 2);
            modelBuilder.Entity<Promocion>().Property(p => p.DescuentoPorcentaje).HasPrecision(5, 2);
        }
    }
}

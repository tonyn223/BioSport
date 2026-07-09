using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BioSport.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Administrador", NormalizedName = "ADMINISTRADOR" },
                new IdentityRole { Id = "2", Name = "Recepcionista", NormalizedName = "RECEPCIONISTA" },
                new IdentityRole { Id = "3", Name = "Entrenador", NormalizedName = "ENTRENADOR" },
                new IdentityRole { Id = "4", Name = "Cliente", NormalizedName = "CLIENTE" }
            );
        }
    }
}
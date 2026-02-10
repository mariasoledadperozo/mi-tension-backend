using mi_tension_backend.Enums;
using mi_tension_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mi_tension_backend.Context
{
    public class MiDbContext : DbContext
    {
        public MiDbContext(DbContextOptions<MiDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RegistroPresion> RegistroPresion { get; set; }
        public DbSet<Recordatorios> Recordatorios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recordatorios>(entity =>
            {
                entity.Property(e => e.Dias)
                    .HasConversion(
                        v => string.Join(',', v.Select(d => d.ToString())),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(d => (DiasSemana)Enum.Parse(typeof(DiasSemana), d))
                              .ToList() 
                    );
            });
        }
    }
}
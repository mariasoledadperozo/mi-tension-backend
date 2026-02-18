using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using mi_tension_backend.Models;
using mi_tension_backend.Enums;

namespace mi_tension_backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Recordatorio> Recordatorio { get; set; } = default!;
        public DbSet<RegistroPresion> RegistroPresion { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de relaciones
            builder.Entity<Recordatorio>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Recordatorios)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RegistroPresion>()
                .HasOne(rp => rp.Usuario)
                .WithMany(u => u.RegistrosPresion)
                .HasForeignKey(rp => rp.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversión del enum List<DiasSemana> a string
            builder.Entity<Recordatorio>()
                .Property(r => r.Dias)
                .HasConversion(
                    v => string.Join(',', v.Select(d => (int)d)),
                    v => string.IsNullOrEmpty(v)
                        ? new List<DiasSemana>()
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(d => (DiasSemana)int.Parse(d))
                           .ToList()
                );

            // Índices para mejorar rendimiento
            builder.Entity<RegistroPresion>()
                .HasIndex(rp => rp.Fecha);

            builder.Entity<Recordatorio>()
                .HasIndex(r => r.Activo);
        }
    }
}
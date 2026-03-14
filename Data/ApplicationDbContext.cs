// Author: María Soledad Perozo
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using mi_tension_backend.Models;
using mi_tension_backend.Enums;

namespace mi_tension_backend.Data
{
    /// <summary>
    /// Contexto de la base de datos para la aplicación, integrando Identity para la gestión de usuarios.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Tabla de usuarios (extendida de Identity).
        /// </summary>
        public DbSet<Usuario> Usuario { get; set; }

        /// <summary>
        /// Tabla de recordatorios de medicación.
        /// </summary>
        public DbSet<Recordatorio> Recordatorio { get; set; } = default!;

        /// <summary>
        /// Tabla de registros de presión arterial.
        /// </summary>
        public DbSet<RegistroPresion> RegistroPresion { get; set; } = default!;

        /// <summary>
        /// Configura el modelo y las relaciones de la base de datos mediante Fluent API.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación Usuario -> Recordatorios (1:N)
            builder.Entity<Recordatorio>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Recordatorios)
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Usuario -> Registros de Presión (1:N)
            builder.Entity<RegistroPresion>()
                .HasOne(rp => rp.Usuario)
                .WithMany(u => u.RegistrosPresion)
                .HasForeignKey(rp => rp.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversión de la lista de enums DiasSemana a una cadena para su almacenamiento en BD
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

            // Índices para optimizar búsquedas frecuentes
            builder.Entity<RegistroPresion>()
                .HasIndex(rp => rp.Fecha);

            builder.Entity<Recordatorio>()
                .HasIndex(r => r.Activo);
        }
    }
}
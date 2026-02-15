using System.ComponentModel.DataAnnotations;
using mi_tension_backend.Enums;

namespace mi_tension_backend.DTOs.Usuario
{
    public class UpdateUsuarioDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        public DateOnly FechaNacimiento { get; set; }

        [Required]
        public Sexo Sexo { get; set; }

        public bool? TomaMedicacion { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}

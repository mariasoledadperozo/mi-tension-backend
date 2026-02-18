using mi_tension_backend.Enums;

namespace mi_tension_backend.DTOs.Usuario
{
    public class UsuarioResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public Sexo Sexo { get; set; }
        public bool? TomaMedicacion { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

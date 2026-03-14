// Author: María Soledad Perozo
using mi_tension_backend.Enums;

namespace mi_tension_backend.DTOs.Usuario
{
    /// <summary>
    /// DTO de respuesta con la información pública del usuario.
    /// </summary>
    public class UsuarioResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public DateOnly FechaNacimiento { get; set; }
        public Sexo Sexo { get; set; }

        public bool? TomaMedicacion { get; set; }
    }
}

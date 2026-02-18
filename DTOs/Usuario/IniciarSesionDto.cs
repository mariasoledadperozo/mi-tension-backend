using System.ComponentModel.DataAnnotations;

namespace mi_tension_backend.DTOs.Usuario
{
    public class IniciarSesionDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
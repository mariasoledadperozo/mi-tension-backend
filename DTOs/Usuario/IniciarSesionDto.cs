// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;

namespace mi_tension_backend.DTOs.Usuario
{
    /// <summary>
    /// DTO para el inicio de sesión de un usuario existente.
    /// </summary>
    public class IniciarSesionDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
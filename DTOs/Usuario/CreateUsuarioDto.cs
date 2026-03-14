// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;

using mi_tension_backend.Attributes;
using mi_tension_backend.Enums;


namespace mi_tension_backend.DTOs.Usuario
{
    /// <summary>
    /// DTO para la creación de un nuevo usuario con credenciales de acceso.
    /// </summary>
    public class CreateUsuarioDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [MaxLength(100)]
        public required string Apellidos { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [NoFutureDate(ErrorMessage = "La fecha de nacimiento no puede ser futura")]
        public required DateOnly FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public required Sexo Sexo { get; set; }


        public bool? TomaMedicacion { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido")]
        public string? PhoneNumber { get; set; }
    }
}

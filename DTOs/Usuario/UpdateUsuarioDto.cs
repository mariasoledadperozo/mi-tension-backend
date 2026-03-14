// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;

using mi_tension_backend.Attributes;
using mi_tension_backend.Enums;


namespace mi_tension_backend.DTOs.Usuario
{
    /// <summary>
    /// DTO para la actualización de datos personales del usuario.
    /// </summary>
    public class UpdateUsuarioDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [NoFutureDate(ErrorMessage = "La fecha de nacimiento no puede ser futura")]
        public DateOnly FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public Sexo Sexo { get; set; }


        public bool? TomaMedicacion { get; set; }
    }
}

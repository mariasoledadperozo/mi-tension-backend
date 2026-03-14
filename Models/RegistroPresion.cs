// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mi_tension_backend.Models
{
    /// <summary>
    /// Representa una toma individual de presión arterial realizada por un usuario.
    /// </summary>
    public class RegistroPresion
    {
        /// <summary>
        /// Identificador único del registro.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID del usuario que realizó la toma.
        /// </summary>
        [Required]
        [ForeignKey("Usuario")]
        public required string UsuarioId { get; set; }

        /// <summary>
        /// Referencia al objeto de navegación del usuario.
        /// </summary>
        [JsonIgnore]
        public Usuario Usuario { get; set; } = null!;

        /// <summary>
        /// Valor de la presión sistólica (máxima) en mmHg.
        /// </summary>
        [Required]
        [Range(30, 300, ErrorMessage = "La presión sistólica debe estar entre 30 y 300 mmHg")]
        public int Sistolica { get; set; }

        /// <summary>
        /// Valor de la presión diastólica (mínima) en mmHg.
        /// </summary>
        [Required]
        [Range(30, 200, ErrorMessage = "La presión diastólica debe estar entre 30 y 200 mmHg")]
        public int Diastolica { get; set; }

        /// <summary>
        /// Frecuencia cardíaca o pulso en lpm.
        /// </summary>
        [Range(30, 220, ErrorMessage = "El pulso debe estar entre 30 y 220 lpm")]
        public int Pulso { get; set; }

        /// <summary>
        /// Fecha y hora en la que se realizó la toma.
        /// </summary>
        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Observaciones adicionales o síntomas durante la toma.
        /// </summary>
        [MaxLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }
    }
}
// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using mi_tension_backend.Enums;

namespace mi_tension_backend.Models
{
    /// <summary>
    /// Representa un recordatorio de medicación para un usuario del sistema.
    /// </summary>
    public class Recordatorio
    {
        /// <summary>
        /// Identificador único del recordatorio.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la medicina o fármaco.
        /// </summary>
        [Required(ErrorMessage = "El nombre de la medicina es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public required string NombreMedicina { get; set; }

        /// <summary>
        /// Dosis recomendada (ej. 50mg, 1 comprimido).
        /// </summary>
        [Required(ErrorMessage = "La dosis es obligatoria")]
        [MaxLength(50, ErrorMessage = "La dosis no puede exceder 50 caracteres")]
        public required string Dosis { get; set; }

        /// <summary>
        /// Hora programada para la toma de la medicación.
        /// </summary>
        [Required(ErrorMessage = "La hora es obligatoria")]
        public required TimeOnly Hora { get; set; }

        /// <summary>
        /// Lista de días de la semana en los que se debe realizar la toma.
        /// </summary>
        public List<DiasSemana> Dias { get; set; } = new();

        /// <summary>
        /// ID del usuario asociado a este recordatorio.
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
        /// Fecha y hora de creación del registro.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica si el recordatorio está activo para enviar notificaciones.
        /// </summary>
        public bool Activo { get; set; } = true;
    }
}
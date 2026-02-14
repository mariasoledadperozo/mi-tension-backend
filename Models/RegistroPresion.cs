using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mi_tension_backend.Models
{
    public class RegistroPresion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public required string UsuarioId { get; set; }

        [JsonIgnore]
        public Usuario Usuario { get; set; } = null!;

        [Required]
        [Range(30, 300, ErrorMessage = "La presión sistólica debe estar entre 30 y 300 mmHg")]
        public int Sistolica { get; set; }

        [Required]
        [Range(30, 200, ErrorMessage = "La presión diastólica debe estar entre 30 y 200 mmHg")]
        public int Diastolica { get; set; }

        [Range(30, 220, ErrorMessage = "El pulso debe estar entre 30 y 220 lpm")]
        public int? Pulso { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [MaxLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        public string? Notas { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using mi_tension_backend.Enums;

namespace mi_tension_backend.Models
{
    /// 
    /// Representa un recordatorio de medicación para un usuario
    /// 
    public class Recordatorio
    {
        [Key]
        public int Id { get; set; }

        /***************************/
        /**   DATOS MEDICACIÓN    **/
        /***************************/

        [Required(ErrorMessage = "El nombre de la medicina es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public required string NombreMedicina { get; set; }

        [Required(ErrorMessage = "La dosis es obligatoria")]
        [MaxLength(50, ErrorMessage = "La dosis no puede exceder 50 caracteres")]
        public required string Dosis { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        public required TimeOnly Hora { get; set; }
        public List<DiasSemana> Dias { get; set; } = new();

        /***************************/
        /**    RELACIÓN USUARIO   **/
        /***************************/

        [Required]
        [ForeignKey("Usuario")]  
        public required string UsuarioId { get; set; }

        [JsonIgnore]
        public Usuario Usuario { get; set; } = null!;

        /***************************/
        /**     AUDITORÍA         **/
        /***************************/

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool Activo { get; set; } = true;
    }
}
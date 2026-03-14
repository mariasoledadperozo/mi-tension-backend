// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;
using mi_tension_backend.Enums;

namespace mi_tension_backend.DTOs.Recordatorio
{
    /// <summary>
    /// DTO para actualizar los datos de un recordatorio existente.
    /// </summary>
    public class UpdateRecordatorioDto
    {
        [Required(ErrorMessage = "El nombre de la medicina es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public required string NombreMedicina { get; set; }

        [Required(ErrorMessage = "La dosis es obligatoria")]
        [MaxLength(50, ErrorMessage = "La dosis no puede exceder 50 caracteres")]
        public required string Dosis { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria")]
        public required TimeOnly Hora { get; set; }

        public List<DiasSemana> Dias { get; set; } = new();

        public bool Activo { get; set; }
    }
}

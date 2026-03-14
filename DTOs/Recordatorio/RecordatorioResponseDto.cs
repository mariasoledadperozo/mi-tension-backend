// Author: María Soledad Perozo
using mi_tension_backend.Enums;

namespace mi_tension_backend.DTOs.Recordatorio
{
    /// <summary>
    /// DTO de respuesta que contiene los detalles de un recordatorio.
    /// </summary>
    public class RecordatorioResponseDto
    {
        public int Id { get; set; }
        public required string NombreMedicina { get; set; }
        public required string Dosis { get; set; }
        public required TimeOnly Hora { get; set; }
        public List<DiasSemana> Dias { get; set; } = new();
        public required string UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}

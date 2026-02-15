using mi_tension_backend.Enums;

namespace mi_tension_backend.DTOs.Recordatorio
{
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

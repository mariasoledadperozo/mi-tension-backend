namespace mi_tension_backend.DTOs.RegistroPresion;

public class RegistroPresionResponseDto
{
    public int Id { get; set; }
    public string UsuarioId { get; set; }
    public int Sistolica { get; set; }
    public int Diastolica { get; set; }
    public int Pulso { get; set; }
    public DateTime Fecha { get; set; }
    public string? Notas { get; set; }
}

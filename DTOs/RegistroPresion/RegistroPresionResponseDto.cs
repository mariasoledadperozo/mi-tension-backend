// Author: María Soledad Perozo
namespace mi_tension_backend.DTOs.RegistroPresion;

/// <summary>
/// DTO de respuesta que contiene los detalles de un registro de presión arterial.
/// </summary>
public class RegistroPresionResponseDto
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public int Sistolica { get; set; }
    public int Diastolica { get; set; }
    public int Pulso { get; set; }
    public DateTime Fecha { get; set; }
    public string? Notas { get; set; }
}

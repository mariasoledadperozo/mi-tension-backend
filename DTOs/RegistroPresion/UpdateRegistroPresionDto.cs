// Author: María Soledad Perozo
namespace mi_tension_backend.DTOs.RegistroPresion;

/// <summary>
/// DTO para actualizar los datos de un registro de presión arterial existente.
/// </summary>
public class UpdateRegistroPresionDto
{
    public int Sistolica { get; set; }
    public int Diastolica { get; set; }
    public int Pulso { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Notas { get; set; }
}

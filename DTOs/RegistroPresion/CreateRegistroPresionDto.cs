// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;
using mi_tension_backend.Attributes;

namespace mi_tension_backend.DTOs.RegistroPresion;

/// <summary>
/// DTO para la creación de un nuevo registro de presión arterial.
/// </summary>
public class CreateRegistroPresionDto
{
    public string UsuarioId { get; set; } = string.Empty;
    public int Sistolica { get; set; }
    public int Diastolica { get; set; }
    public int Pulso { get; set; }

    [NoFutureDate(ErrorMessage = "La fecha de la toma no puede ser futura")]
    public DateTime? Fecha { get; set; }
    public string? Notas { get; set; }
}

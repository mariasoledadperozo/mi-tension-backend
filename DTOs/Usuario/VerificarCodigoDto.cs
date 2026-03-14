// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;

namespace mi_tension_backend.DTOs.Usuario
{
    public class VerificarCodigoDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Codigo { get; set; } = string.Empty;
    }
}

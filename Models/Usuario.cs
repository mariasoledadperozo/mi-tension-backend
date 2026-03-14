// Author: María Soledad Perozo
using mi_tension_backend.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace mi_tension_backend.Models
{
    /// <summary>
    /// Representa a un usuario en el sistema, extendiendo la funcionalidad base de ASP.NET Core Identity.
    /// </summary>
    public class Usuario : IdentityUser
    {
        /// <summary>
        /// Nombre de pila del usuario.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellidos del usuario.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        [Required]
        public DateOnly FechaNacimiento { get; set; }

        /// <summary>
        /// Sexo biológico del usuario.
        /// </summary>
        [Required]
        public Sexo Sexo { get; set; }


        /// <summary>
        /// Indica si el usuario toma medicación para la tensión habitualmente.
        /// </summary>
        public bool? TomaMedicacion { get; set; }

        /// <summary>
        /// Colección de registros de presión arterial del usuario.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<RegistroPresion> RegistrosPresion { get; set; }
            = new List<RegistroPresion>();

        /// <summary>
        /// Código de verificación de 6 dígitos para el registro.
        /// </summary>
        public string? CodigoVerificacion { get; set; }

        /// <summary>
        /// Fecha y hora en que expira el código de verificación.
        /// </summary>
        public DateTime? CodigoVerificacionExpiracion { get; set; }

        /// <summary>
        /// Colección de recordatorios de medicación del usuario.
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Recordatorio> Recordatorios { get; set; }
            = new List<Recordatorio>();
    }
}
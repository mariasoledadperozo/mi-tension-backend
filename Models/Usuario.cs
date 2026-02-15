using mi_tension_backend.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace mi_tension_backend.Models
{
    /// <summary>
    /// Clase Usuario que representa a un usuario en el sistema.
    /// Hereda de IdentityUser para autenticación y gestión de usuarios.
    /// </summary>
    public class Usuario : IdentityUser
    {
        /***************************/
        /**   DATOS DE USUARIO    **/
        /***************************/

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        public DateOnly FechaNacimiento { get; set; }

        [Required]
        public Sexo Sexo { get; set; }

        public bool? TomaMedicacion { get; set; }

        /***************************/
        /**   RELACIONES          **/
        /***************************/

        [JsonIgnore]
        public virtual ICollection<RegistroPresion> RegistrosPresion { get; set; }
            = new List<RegistroPresion>();
        [JsonIgnore]
        public virtual ICollection<Recordatorio> Recordatorios { get; set; }
            = new List<Recordatorio>();
    }
}
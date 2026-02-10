using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using mi_tension_backend.Enums; 

namespace mi_tension_backend.Models
{
    public class Recordatorios
    {
        [Key]
        public int id { get; set; }

        public required string nombreMedicina { get; set; }
        public required string dosis { get; set; }
        public required TimeOnly hora { get; set; }

        public List<DiasSemana> Dias { get; set; } = new();

        public required Guid userId { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
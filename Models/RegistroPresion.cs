using System.ComponentModel.DataAnnotations.Schema;
using mi_tension_backend.Models; 

namespace mi_tension_backend.Models 
{
    public class RegistroPresion
    {
        public int id { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public Usuario? Usuario { get; set; } 

        public int Sistolica { get; set; }
        public int Diastolica { get; set; }
        public DateTime Fecha { get; set; } 
    }
}
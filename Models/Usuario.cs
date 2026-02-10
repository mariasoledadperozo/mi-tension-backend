public class Usuario
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty; 
    public bool EsFumador { get; set; }
    public bool TomaMedicacion { get; set; }
    public double? Altura { get; set; } 
    public double? Peso { get; set; }  

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
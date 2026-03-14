// Author: María Soledad Perozo
namespace mi_tension_backend.Services
{
    /// <summary>
    /// Configuración para las credenciales y ajustes del servicio de correo electrónico.
    /// </summary>
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
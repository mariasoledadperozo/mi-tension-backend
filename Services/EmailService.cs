// Author: María Soledad Perozo
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net;

namespace mi_tension_backend.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var client = new SendGridClient(_emailSettings.Password);
            var from = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, plainTextContent: null, htmlContent: htmlBody);

            var response = await client.SendEmailAsync(msg);
            var responseBody = await response.Body.ReadAsStringAsync();

            Console.WriteLine($"[SendGrid] Status: {response.StatusCode}");
            Console.WriteLine($"[SendGrid] Body: {responseBody}");

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"SendGrid error {response.StatusCode}: {responseBody}");
            }

            Console.WriteLine($"[EmailService] Correo enviado exitosamente a {to}");
        }
    }
}
// Author: María Soledad Perozo
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace mi_tension_backend.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"] ?? "mperozo241000@gmail.com";

            var payload = new
            {
                personalizations = new[]
                {
                    new { to = new[] { new { email = to } } }
                },
                from = new { email = fromEmail, name = "Mi Tensión" },
                subject = subject,
                content = new[]
                {
                    new { type = "text/html", value = htmlBody }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"SendGrid error {response.StatusCode}: {error}");
            }
        }
    }
}
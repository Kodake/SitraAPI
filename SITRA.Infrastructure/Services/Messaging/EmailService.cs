using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SITRA.Core.Interfaces.Services.Messaging;
using System.Net;
using System.Net.Mail;

namespace SITRA.Infrastructure.Services.Messaging
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            var host = _configuration["Smtp:Host"];

            // If no SMTP host is configured, operate in Test Mode (Console Simulation)
            if (string.IsNullOrWhiteSpace(host))
            {
                var match = System.Text.RegularExpressions.Regex.Match(body, @"<div class='code'>(\d{6})</div>");
                var otp = match.Success ? match.Groups[1].Value : "N/A";

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║            [SITRA RECUPERACIÓN DE CONTRASEÑA - TEST MODE]            ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════════════════════╣");
                Console.WriteLine($"║ Para (To)    : {to,-54}║");
                Console.WriteLine($"║ Asunto       : {subject,-54}║");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("╠══════════════════════════════════════════════════════════════════════╣");
                Console.WriteLine($"║ >>> CÓDIGO DE VERIFICACIÓN OTP: {otp} <<< (Válido: 15 minutos)      ║");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╠══════════════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║ NOTA: Para recibir el correo real en tu bandeja de entrada,          ║");
                Console.WriteLine("║ ingresa tus credenciales SMTP en 'Smtp' dentro de appsettings.json.  ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                _logger.LogInformation("[EMAIL SERVICE - TEST MODE] Verification code for {To}: {Otp}", to, otp);
                return;
            }

            try
            {
                var port = int.TryParse(_configuration["Smtp:Port"], out var p) ? p : 587;
                var enableSsl = !bool.TryParse(_configuration["Smtp:EnableSsl"], out var ssl) || ssl;
                var username = _configuration["Smtp:Username"];
                var password = _configuration["Smtp:Password"];
                var fromEmail = _configuration["Smtp:FromEmail"] ?? "noreply@sitrapower.com";
                var fromName = _configuration["Smtp:FromName"] ?? "SITRA POS";

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                if (!string.IsNullOrEmpty(username))
                {
                    client.Credentials = new NetworkCredential(username, password);
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(to);

                await client.SendMailAsync(message, cancellationToken);
                _logger.LogInformation("Email successfully dispatched via SMTP to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via SMTP to {To}. Operating in simulation fallback.", to);
                // Also log to console in case SMTP is unreachable so testing isn't blocked
                _logger.LogWarning(
                    "\n=== EMAIL FALLBACK LOG ===\nTo: {To}\nSubject: {Subject}\nBody:\n{Body}\n==========================",
                    to, subject, body);
            }
        }
    }
}


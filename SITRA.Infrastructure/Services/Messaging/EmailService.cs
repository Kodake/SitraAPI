using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using Microsoft.Extensions.Logging;
using SITRA.Core.Interfaces.Services;

namespace SITRA.Infrastructure.Services.Messaging
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            // For now, we simulate sending an email by logging it to the console.
            // When you are ready to configure a real SMTP provider (e.g. SendGrid, Office365),
            // you can instantiate an SmtpClient here and send the message.
            _logger.LogInformation("========== EMAIL DISPATCH SIMULATION ==========");
            _logger.LogInformation($"To: {to}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body:\n{body}");
            _logger.LogInformation("===============================================");
            
            return Task.CompletedTask;
        }
    }
}

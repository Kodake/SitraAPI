namespace SITRA.Core.Interfaces.Services.Messaging
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
}

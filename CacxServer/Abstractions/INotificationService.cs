namespace CacxServer.Abstractions;

public interface INotificationService
{
    Task<bool> SendEmailAsync(IEnumerable<string> targetEmails, string subject, string body);
    Task SendSmsAsync();
}
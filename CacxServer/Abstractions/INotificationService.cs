namespace CacxServer.Abstractions;

public interface INotificationService
{
    Task SendEmailAsync(IEnumerable<string> targetEmails, string subject, string body);
    Task SendSmsAsync();
}

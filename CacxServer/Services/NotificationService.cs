using CacxServer.Abstractions;
using DotNetEnv;
using System.Net;
using System.Net.Mail;

namespace CacxServer.Services;

public sealed class NotificationService : INotificationService
{
    public async Task SendEmailAsync(IEnumerable<string> targetEmails, string subject, string body)
    {
        Console.WriteLine($"CODE[REMOVE THIS CW IN NOTIFICATIONSERVICE AFTER TESTING]: {body}");
        return;

        const string EmailAttribute = "EMAIL";
        const string AppPasswordAttribute = "EMAIL_APP_PASSWORD";

        MailMessage mail = new()
        {
            From = new MailAddress(address: Env.GetString(EmailAttribute)),
            Subject = subject,
            Body = body,
        };
        
        foreach (string email in targetEmails)
        {
            try
            {
                mail.To.Add(new MailAddress(email));
            }
            catch {/*/ Skip an email if it´s in the wrong format/*/ }
        }

        using SmtpClient smtp = new(host: "smtp.gmail.com", port: 587)
        {
            Credentials = new NetworkCredential(userName: Env.GetString(EmailAttribute), password: Env.GetString(AppPasswordAttribute)),
            EnableSsl = true
        };

        await smtp.SendMailAsync(mail);
    }

    public async Task SendSmsAsync()
    {
        throw new NotImplementedException(nameof(SendSmsAsync));
    }
}

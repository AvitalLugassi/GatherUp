using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Email;

public class EmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody)
    {
        // TODO: מימוש עם SMTP / SendGrid
        Console.WriteLine($"[EMAIL] To: {to} | Subject: {subject}");
        return Task.CompletedTask;
    }

    public Task SendBulkAsync(IEnumerable<string> recipients, string subject, string htmlBody)
    {
        foreach (var r in recipients)
            Console.WriteLine($"[EMAIL BULK] To: {r} | Subject: {subject}");
        return Task.CompletedTask;
    }
}

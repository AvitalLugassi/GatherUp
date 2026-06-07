using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Email;

public class EmailService : IEmailService
{
    public void Send(string to, string subject, string htmlBody)
    {
        // TODO: מימוש עם SMTP / SendGrid
        Console.WriteLine($"[EMAIL] To: {to} | Subject: {subject}");
    }

    public void SendBulk(IEnumerable<string> recipients, string subject, string htmlBody)
    {
        foreach (var r in recipients)
            Console.WriteLine($"[EMAIL BULK] To: {r} | Subject: {subject}");
    }
}


using backend.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace backend.Services
{
public class EmailService
{
    private readonly IOptions<EmailSettings> _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(_emailSettings.Value.SenderName, _emailSettings.Value.Sender));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = message };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_emailSettings.Value.MailServer, _emailSettings.Value.MailPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Value.Sender, _emailSettings.Value.Password);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}

}
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace WallsShop.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
}

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("WallsShop Development", _settings.FromEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            // Connect to Gmail using STARTTLS on Port 587
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);

            // Authenticate with your Gmail and App Password
            await smtp.AuthenticateAsync(_settings.User, _settings.Pass);

            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to send email.", ex);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}


public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string User { get; set; } = string.Empty;
    public string Pass { get; set; } = string.Empty; // Use Gmail App Password here
    public string FromEmail { get; set; } = string.Empty;
}
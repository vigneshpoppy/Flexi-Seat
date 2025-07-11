using FlexiSeat.Models;
using System.Net.Mail;
using System.Net;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace FlexiSeat.Services
{
    public class EmailService : IEmailService
    {
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
      _settings = options.Value;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var message = new MailMessage
            {
              From = new MailAddress(_settings.FromEmail),
              Subject = subject,
              Body = body,
              IsBodyHtml = isHtml
            };

            message.To.Add(toEmail);

            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
              Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPassword),
              EnableSsl = true
            };

            await client.SendMailAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send failed: {ex.Message}");
            return false;
        }
    }
  }
}

using FlexiSeat.Models;
using System.Net.Mail;
using System.Net;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Azure.Communication.Email;
using Azure;

namespace FlexiSeat.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly EmailClient _emailClient;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
            _emailClient = new EmailClient(_settings.ConnectionString);
        }

        public async Task<bool> SendEmailAsync(string toEmails, string ccEmails, string subject, string body, bool isHtml = true)
        {
            try
            {
                var content = new EmailContent(subject)
                {
                    Html = isHtml ? body : null,
                    PlainText = !isHtml ? body : null
                };

                // Parse comma-separated toEmails into EmailAddress list
                var toRecipients = toEmails?
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(email => new EmailAddress(email.Trim()))
                    .ToList() ?? new List<EmailAddress>();

                // Parse comma-separated ccEmails into EmailAddress list
                var ccRecipients = ccEmails?
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(email => new EmailAddress(email.Trim()))
                    .ToList() ?? new List<EmailAddress>();

                if (!toRecipients.Any())
                {
                    Console.WriteLine("No 'To' recipients specified.");
                    return false;
                }

                var recipients = new EmailRecipients(toRecipients, ccRecipients);

                var emailMessage = new EmailMessage(_settings.FromEmail, recipients, content);

                EmailSendOperation sendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                //Console.WriteLine($"Email sent. Message ID: {sendOperation.Id}");
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Email send failed: {ex.Message}");
                return false;
            }
        }
    }
}

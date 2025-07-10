using System.Net.Mail;
using System.Net;

namespace FlexiSeat.Helper
{
    public static class EmailHelper
    {
        private static readonly string SmtpHost = "smtp.yourdomain.com";
        private static readonly int SmtpPort = 587;
        private static readonly string SmtpUser = "your-smtp-username";
        private static readonly string SmtpPass = "your-smtp-password";
        private static readonly string FromEmail = "no-reply@yourdomain.com";

        public static async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(toEmail);

            using var smtpClient = new SmtpClient(SmtpHost, SmtpPort)
            {
                Credentials = new NetworkCredential(SmtpUser, SmtpPass),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

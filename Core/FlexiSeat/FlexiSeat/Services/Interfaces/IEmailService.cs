namespace FlexiSeat.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmails, string ccEmails, string subject, string body, bool isHtml = true);
    }
}

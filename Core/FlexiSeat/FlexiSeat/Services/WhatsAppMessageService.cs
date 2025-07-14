using FlexiSeat.Data;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
namespace FlexiSeat.Services
{
  public class WhatsAppMessageService
  {
    private readonly TwilioSettings _twilioSettings;
    public WhatsAppMessageService(IOptions<TwilioSettings> twilioOptions)
    {
      _twilioSettings = twilioOptions.Value;      
      TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
    }

    public async Task<string> SendWhatsAppMessage(string toPhoneNumber, string message)
    {
      var to = new PhoneNumber("whatsapp:" + toPhoneNumber);
      var from = new PhoneNumber(_twilioSettings.WhatsAppFrom);
      var date = DateTime.Today.AddDays(1).ToString("dd MMM yyyy");
      var msg = MessageResource.Create(
    from: from,
    to: to,
    contentSid: "HX6f948c18fbe16d78400f9665c91693df",
    contentVariables: JsonSerializer.Serialize(new { date })
);

      return msg.Sid;
    }
  }
}

using System;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using FlexiSeat.DbContext;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeatMessager
{
    public class MessagerFunction
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly FlexiSeatDbContext _context;

        public MessagerFunction(ILoggerFactory loggerFactory, IConfiguration config, FlexiSeatDbContext context)
        {
            _logger = loggerFactory.CreateLogger<MessagerFunction>();
            _config = config;
            _context = context;
        }

        [Function("MessagerFunction")]
        public void Run([TimerTrigger("0 09 05 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Timer trigger function started at: {DateTime.Now}");

            string accountSid = _config["TwilioAccountSid"];
            string authToken = _config["TwilioAuthToken"];
            string fromNumber = _config["TwilioFromNumber"];
            string toNumber = _config["TwilioToNumber"];
            string contentSid = _config["TwilioContentSid"];

            string seatNumber = "CS381B";
              //  _context.Employees
              //.Where(r => r.PhoneNumber.Equals(toNumber)).FirstOrDefault().EmployeeADID;
            TwilioClient.Init(accountSid, authToken);

            var tomorrow = DateTime.Today.AddDays(1).ToString("dd MMM yyyy");

            var message = MessageResource.Create(
                from: new PhoneNumber(fromNumber),
                to: new PhoneNumber(toNumber),
                contentSid: contentSid,
                contentVariables: JsonSerializer.Serialize(new
                {
                    date = tomorrow,
                    seatno = seatNumber
                })
            );

            _logger.LogInformation($"Message SID: {message.Sid}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}

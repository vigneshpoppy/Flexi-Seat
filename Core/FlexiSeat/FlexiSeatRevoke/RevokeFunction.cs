using System;
using FlexiSeat.DbContext;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlexiSeatRevoke
{
    public class RevokeFunction
    {
        private readonly ILogger _logger;
        private readonly FlexiSeatDbContext _context;

        public RevokeFunction(ILoggerFactory loggerFactory, FlexiSeatDbContext context)
        {
            _logger = loggerFactory.CreateLogger<RevokeFunction>();
            _context = context;
        }

        [Function("RevokeFunction")]
        public async Task Run([TimerTrigger("0 0 10 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                await _context.Reservations
                    .Where(r => r.Status.Equals("Booked"))
                    .ExecuteDeleteAsync();

                _logger.LogInformation($"Unconfirmed reservation has been deleted: {DateTime.Now}");
            }

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}

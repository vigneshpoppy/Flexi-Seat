using System;
using FlexiSeat.DbContext;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlexiSeatReset
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly FlexiSeatDbContext _context;

        public Function1(ILoggerFactory loggerFactory, FlexiSeatDbContext context)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _context = context;
        }

        [Function("Function1")]
        public void Run([TimerTrigger("0 57 02 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _context.Reservations
              .Where(r => string.IsNullOrEmpty(r.CheckOutTime) && !string.IsNullOrEmpty(r.CheckInTime))
              .ExecuteUpdateAsync(setters => setters
                  .SetProperty(r => r.CheckOutTime, "22:00"));
                _context.SaveChanges();
            }
        }
    }
}

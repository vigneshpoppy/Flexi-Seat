using System;
using FlexiSeat.DbContext;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlexiSeatReset
{
    public class RemoveReservations
    {
        private readonly ILogger _logger;
        private readonly FlexiSeatDbContext _context;

        public RemoveReservations(ILoggerFactory loggerFactory, FlexiSeatDbContext context)
        {
            _logger = loggerFactory.CreateLogger<RemoveReservations>();
            _context = context;
        }

        [Function("RemoveReservations")]
        public void Run([TimerTrigger("0 0 10 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _context.Reservations
              .Where(r => !string.IsNullOrEmpty(r.Status) && r.Status.ToLower() != "booked").ExecuteDelete();
                _context.SaveChanges();
            }
        }
    }
}

using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.DashboardDTOs;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        public DashboardController(FlexiSeatDbContext context)
        {
            _context = context;
        }

        [HttpGet("Statistics")]
        public async Task<IActionResult> GetStatisticsByDate([FromQuery] DateTime date)
        {
            var statsDTO = new StatsDTO();
            statsDTO.DashboardSeatsDTO = await GetSeatDashboardDetailsByDate(date);
            statsDTO.DashboardWeeklyCheckinCount = await GetLast5WorkingDayCheckins(date);
            statsDTO.ZoneDetails = await GetZonesStatusAsync();
            statsDTO.RoleDetails = await GetRolesStatusAsync();
            return Ok(statsDTO);
        }

        private async Task<DashboardSeatsDTO> GetSeatDashboardDetailsByDate(DateTime date)
        {
            // Get active zones
            var activeZoneIds = await _context.Zones
                .Where(z => z.IsActive)
                .Select(z => z.ID)
                .ToListAsync();

            // Get active seats in active zones
            var activeSeats = await _context.Seats
                .Where(s => s.IsActive && activeZoneIds.Contains(s.ZoneId))
                .ToListAsync();

            int totalSeats = activeSeats.Count;

            var seatIds = activeSeats.Select(s => s.ID).ToList();

            // Get reservations for the selected date for these seats
            var reservations = await _context.Reservations
                .Where(r => r.ReservedDate == date && seatIds.Contains(r.SeatID))
                .ToListAsync();

            int checkedInCount = reservations.Count(r => !string.IsNullOrEmpty(r.CheckInTime));
            int confirmedCount = reservations.Count(r => r.Status.ToLower() == "confirmed");

            // Compute available seats by subtracting reserved ones
            var reservedSeatIds = reservations.Select(r => r.SeatID).Distinct().ToHashSet();
            int availableCount = activeSeats.Count(s => !reservedSeatIds.Contains(s.ID));

            string Format(int count) =>
                $"{count},{(totalSeats == 0 ? 0 : (int)Math.Round((double)count * 100 / totalSeats))}%";

            var dto = new DashboardSeatsDTO
            {
                TotalCount = totalSeats.ToString(),
                AvailableCountAndPercentage = Format(availableCount),
                ConfirmedCountAndPercentage = Format(confirmedCount),
                CheckedInCountAndPercentage = Format(checkedInCount),
                Date = date
            };

            return dto;
        }

        private async Task<List<Dictionary<string, string>>> GetLast5WorkingDayCheckins(DateTime date)
        {
            var workingDays = new List<DateTime>();
            var currentDate = date.Date;

            // Collect up to 5 past working days (excluding Sat/Sun)
            while (workingDays.Count < 5)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays.Insert(0, currentDate); // Keep chronological order
                }
                currentDate = currentDate.AddDays(-1);
            }

            // Query reservations that are checked in on those dates
            var reservations = await _context.Reservations
                .Where(r => workingDays.Contains(r.ReservedDate.Date) && !string.IsNullOrEmpty(r.CheckInTime))
                .ToListAsync();

            // Group reservations by date and count
            var groupedCheckins = reservations
                .GroupBy(r => r.ReservedDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            // Build result as list of dictionaries with date and count
            var result = new List<Dictionary<string, string>>();
            foreach (var day in workingDays)
            {
                var count = groupedCheckins.TryGetValue(day, out int value) ? value.ToString() : "0";
                result.Add(new Dictionary<string, string>
        {
            { day.ToString("yyyy-MM-dd"), count }
        });
            }

            return result;
        }

        private async Task<List<Dictionary<string, string>>> GetZonesStatusAsync()
        {
            var zones = await _context.Zones.ToListAsync();

            var zoneStatusList = zones
                .Select(z => new Dictionary<string, string>
                {
            { z.Name, z.IsActive ? "1" : "0" }
                })
                .ToList();

            return zoneStatusList;
        }

        private async Task<List<Dictionary<string, string>>> GetRolesStatusAsync()
        {
            var roles = await _context.Roles.ToListAsync();

            var roleStatusList = roles
                .Select(z => new Dictionary<string, string>
                {
            { z.Name, z.IsActive ? "1" : "0" }
                })
                .ToList();

            return roleStatusList;
        }
    }
}

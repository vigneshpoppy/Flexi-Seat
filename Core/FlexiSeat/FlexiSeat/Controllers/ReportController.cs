using FlexiSeat.DbContext;
using FlexiSeat.DTO.ReportDTO;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ReportController : ControllerBase
  {
    private readonly FlexiSeatDbContext _context;
    private readonly IEmailService _emailService;
    public ReportController(FlexiSeatDbContext context, IEmailService emailService)
    {
      _context = context;
      _emailService = emailService;
    }
    [HttpGet("ZoneOccupancyReport")]
    public async Task<IActionResult> GetZoneOccupancyReport(DateTime startDate, DateTime endDate)
    {
      var result = await _context.Reservations
          .Where(r => r.ReservedDate >= startDate && r.ReservedDate <= endDate)
          .GroupBy(r => r.Seat.ZoneId)
          .Select(g => new ZoneOccupancyReportDTO
          {
            ZoneId = g.Key,
            ZoneName = g.First().Seat.Zone.Name,
            TotalSeats = _context.Seats.Count(s => s.ZoneId == g.Key),
            UniqueBookings = g.Select(x => x.SeatID).Distinct().Count(),
            TotalReservations = g.Count(),
          })
          .ToListAsync();

      return Ok(result);
    }

    [HttpGet("attendance/manager/{managerAdid}")]
    public async Task<IActionResult> GetEmployeeAttendanceByManager(string managerAdid)
    {
      var attendance = await _context.Reservations
          .Include(r => r.Seat)
          .Include(r => r.User)
          .Where(r => r.User.ManagerADID == managerAdid)
          .Select(r => new
          {
            r.UserADID,
            UserName = r.User.Name,
            r.SeatID,
            r.ReservedDate,
            r.CheckInTime,
            r.CheckOutTime,
            TotalHours = CalculateTotalHours(r.CheckInTime, r.CheckOutTime),
            r.Seat.Number,
            r.Seat.ZoneId
          })
          .ToListAsync();

      return Ok(attendance);
    }

    [HttpGet("attendance/manager/GetEmployeeAttendance")]
    public async Task<List<AttendanceReportDTO>> GetEmployeeAttendance(string adid, DateTime weekStart)
    {
      DateTime weekEnd = weekStart.AddDays(6);

      return await _context.Reservations
          .Where(r => r.UserADID == adid && r.ReservedDate >= weekStart && r.ReservedDate <= weekEnd)
          .Select(r => new AttendanceReportDTO
          {
            Date = r.ReservedDate,
            CheckInTime = r.CheckInTime,
            CheckOutTime = r.CheckOutTime,
          //  TotalHours = CalculateTotalHours(r.CheckInTime, r.CheckOutTime),
            IsLateCheckIn = string.Compare(r.CheckInTime, "09:30") > 0,
            IsMissingCheckOut = string.IsNullOrEmpty(r.CheckOutTime)
          })
          .ToListAsync();
    }

    private double? CalculateTotalHours(string? checkInTime, string? checkOutTime)
    {

      if (string.IsNullOrWhiteSpace(checkInTime) || string.IsNullOrWhiteSpace(checkOutTime))
        return 0;

      if (TimeSpan.TryParse(checkInTime, out var inTime) && TimeSpan.TryParse(checkOutTime, out var outTime))
      {
        var duration = outTime - inTime;

        // Handle overnight shift edge case if needed
        if (duration.TotalHours < 0)
          duration += TimeSpan.FromHours(24);

        return duration.TotalHours;
      }

      return 0;
    }

    [HttpGet("GetOverstayedReservations")]
    public async Task<List<OverstayReportDTO>> GetOverstayedReservations(DateTime date)
    {
      return await _context.Reservations
          .Where(r => r.ReservedDate == date && r.CheckOutTime == null)
          .Select(r => new OverstayReportDTO
          {
            ADID = r.UserADID,
            SeatNumber = r.Seat.Number,
            ReservedDate = r.ReservedDate,
            CheckInTime = Convert.ToDateTime(r.CheckInTime)
          })
          .ToListAsync();
    }

    [HttpGet("GetNoShowReport")]
    public async Task<List<NoShowReportDTO>> GetNoShowReport(DateTime date)
    {
      return await _context.Reservations
          .Where(r => r.ReservedDate == date && string.IsNullOrEmpty(r.CheckInTime))
          .Select(r => new NoShowReportDTO
          {
            ADID = r.UserADID,
            SeatID = r.SeatID,
            ReservedDate = r.ReservedDate
          })
          .ToListAsync();
    }
    [HttpGet("GetManagerWiseUsage")]
    public async Task<List<ManagerUsageReportDTO>> GetManagerWiseUsage(DateTime start, DateTime end)
    {
      return await _context.Users
          .Where(u => u.ManagerADID != null)
          .GroupBy(u => u.ManagerADID)
          .Select(g => new ManagerUsageReportDTO
          {
            ManagerADID = g.Key,
            EmployeeCount = g.Count(),
            TotalReservations = _context.Reservations
                  .Where(r => g.Select(x => x.ADID).Contains(r.UserADID) &&
                              r.ReservedDate >= start && r.ReservedDate <= end)
                  .Count()
          }).ToListAsync();
    }
    [HttpGet("GetLiveSeatStatus")]
    public async Task<List<LiveSeatStatusDTO>> GetLiveSeatStatus()
    {
      var today = DateTime.Today;

      var reservedSeats = await _context.Reservations
          .Where(r => r.ReservedDate == today)
          .Select(r => r.SeatID)
          .ToListAsync();

      return await _context.Seats
          .Select(s => new LiveSeatStatusDTO
          {
            SeatId = s.ID,
            SeatNumber = s.Number,
            Zone = s.Zone.Name,
            IsReserved = reservedSeats.Contains(s.ID)
          }).ToListAsync();
    }
    [HttpGet("GetLunchEstimation")]
    public async Task<int> GetLunchEstimation(DateTime date)
    {
      return await _context.Reservations
          .Where(r => r.ReservedDate == date && string.Compare(r.CheckInTime, "11:00") <= 0)
          .CountAsync();
    }


  }
}

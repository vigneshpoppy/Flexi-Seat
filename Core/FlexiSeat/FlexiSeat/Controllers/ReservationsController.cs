using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.ReservationDTOs;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        public ReservationsController(FlexiSeatDbContext context, IEmailService emailService)
        {
          _context = context;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDTO dto)
        {
            if (!ModelState.IsValid)
              return BadRequest(ModelState);

            // Normalize ADIDs
            string userAdid = dto.UserADID.Trim().ToUpper();
            string reservedByAdid = dto.ReservedByADID.Trim().ToUpper();

            // Check if User exists
            var user = await _context.Users.FindAsync(userAdid);
            if (user == null)
              return NotFound($"User with ADID '{userAdid}' not found.");

            // Check if ReservedBy user exists
            var reservedBy = await _context.Users.FindAsync(reservedByAdid);
            if (reservedBy == null)
              return NotFound($"ReservedBy user with ADID '{reservedByAdid}' not found.");

            // Check if Seat exists
            var seat = await _context.Seats
                       .FirstOrDefaultAsync(s => s.ID == dto.SeatID && s.IsActive);

            if (seat == null)
              return NotFound($"Active seat with ID '{dto.SeatID}' not found.");

            // Optional: Check if this seat is already reserved for this date
            bool alreadyReserved = await _context.Reservations
                .AnyAsync(r => r.SeatID == dto.SeatID && r.ReservedDate == dto.ReservedDate);
            if (alreadyReserved)
              return Conflict("Seat is already reserved for the selected date.");

            // Create reservation entity
            var reservation = new Reservation
            {
              UserADID = userAdid,
              SeatID = dto.SeatID,
              ReservedDate = dto.ReservedDate.Date,
              InsertedOn = DateTime.Now,
              IsNotified = false,
              ReservedByADID = reservedByAdid
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reservation created successfully.", reservation.ID });
        }

        [HttpGet("AvailableZones")]
        public async Task<IActionResult> GetAvailableZones([FromQuery] string adid, [FromQuery] DateTime date)
        {
          if (string.IsNullOrWhiteSpace(adid))
            return BadRequest("ADID is required.");

          string normalizedAdid = adid.Trim().ToUpper();

          // Get the user
          var user = await _context.Users.FirstOrDefaultAsync(u => u.ADID == normalizedAdid);
          if (user == null)
            return NotFound("User not found.");

          string? managerAdid = user.ManagerADID;

          // Get all seat IDs reserved for the given date
          var reservedSeatIds = await _context.Reservations
              .Where(r => r.ReservedDate.Date == date.Date)
              .Select(r => r.SeatID)
              .ToListAsync();

          // Get all active and unreserved seats
          var availableSeats = await _context.Seats
              .Where(s => s.IsActive && !reservedSeatIds.Contains(s.ID))
              .Include(s => s.Zone)
              .ToListAsync();

          if (!availableSeats.Any() && availableSeats != null)
            return NoContent(); // No available seats

          // Group seats by ZoneId
          var seatGroups = availableSeats
              .GroupBy(s => s.ZoneId)
              .ToDictionary(g => g.Key, g => g.Count());

          List<Zone> zones;
          Dictionary<int, int> seatCounts = seatGroups;

          if (!string.IsNullOrEmpty(managerAdid))
          {
            // Get zones mapped to manager via OrgSeatPool
            var managerZoneIds = await _context.OrgSeatPools
                .Where(p => p.ManagerADID == managerAdid)
                .Select(p => p.ZoneId)
                .ToListAsync();

            // Filter zones that are active and have available seats
            zones = await _context.Zones
                .Where(z => z.IsActive && managerZoneIds.Contains(z.ID) && seatCounts.Keys.Contains(z.ID))
                .ToListAsync();

            // If manager zones have no available seats or all inactive, fallback to all active zones with seats
            if (!zones.Any())
            {
              zones = await _context.Zones
                  .Where(z => z.IsActive && seatCounts.Keys.Contains(z.ID))
                  .ToListAsync();
            }
          }
          else
          {
            // No manager, get all active zones with available seats
            zones = await _context.Zones
                .Where(z => z.IsActive && seatCounts.Keys.Contains(z.ID))
                .ToListAsync();
          }

          var result = zones.Select(z => new GetReservationZoneDTO
          {
            ID = z.ID,
            Name = z.Name,
            Description = z.Description,
            IsActive = z.IsActive,
            ManagerADID = _context.OrgSeatPools.FirstOrDefault(p => p.ZoneId == z.ID)?.ManagerADID,
            ManagerName = _context.OrgSeatPools
                  .Include(p => p.Manager)
                  .FirstOrDefault(p => p.ZoneId == z.ID)?.Manager?.Name,
            AvailableSeatCount = seatCounts.ContainsKey(z.ID) ? seatCounts[z.ID] : 0
          }).ToList();

          return Ok(result);
        }

        [HttpGet("zone/{zoneId}/seats")]
        public async Task<IActionResult> GetSeatsByZoneAndDate(int zoneId, [FromQuery] DateTime date)
        {
          // Validate zone
          var zone = await _context.Zones.FirstOrDefaultAsync(z => z.ID == zoneId);
          if (zone == null)
            return NotFound($"Zone with ID {zoneId} not found.");

          // Get manager info from OrgSeatPool
          var orgSeatPool = await _context.OrgSeatPools
              .Include(p => p.Manager)
              .FirstOrDefaultAsync(p => p.ZoneId == zoneId);

          string? managerAdid = orgSeatPool?.ManagerADID;
          string? managerName = orgSeatPool?.Manager?.Name;

          // Get seat IDs booked on given date in this zone
          var bookedSeatIds = await _context.Reservations
              .Where(r => r.ReservedDate.Date == date.Date && r.Seat.ZoneId == zoneId)
              .Select(r => r.SeatID)
              .ToListAsync();

          // Get all seats in the zone
          var seats = await _context.Seats
              .Where(s => s.ZoneId == zoneId)
              .ToListAsync();

          var result = seats.Select(s => new GetReservationSeatDTO
          {
            ID = s.ID,
            Number = s.Number,
            ZoneId = s.ZoneId,
            ZoneName = zone.Name,
            ManagerADID = managerAdid,
            ManagerName = managerName,
            isBooked = bookedSeatIds.Contains(s.ID),
            IsActive = s.IsActive
          }).ToList();

          return Ok(result);
        }
    }
}

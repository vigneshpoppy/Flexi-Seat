using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly FlexiSeatDbContext _context;

        public ReservationController(ILogger<ReservationController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //GetZoneswithAvailableSeats with date

        [HttpGet("zones")]
        public async Task<IActionResult> GetReservationZones([FromQuery] string userAdid, [FromQuery] DateTime date)
        {
            if (string.IsNullOrEmpty(userAdid))
                return BadRequest("User ADID is required.");

            var user = await _context.Users
                .Include(u => u.Manager)
                .FirstOrDefaultAsync(u => u.ADID == userAdid);

            if (user == null)
                return NotFound("User not found.");

            var managerAdid = user.ManagerADID;

            if (string.IsNullOrEmpty(managerAdid))
                return BadRequest("User has no manager assigned.");

            // Get zones where manager is assigned
            var managerZones = await _context.Zones
                .Where(z => z.ManagerADID == managerAdid)
            .ToListAsync();

            var activeManagerZones = managerZones.Where(z => z.IsActive)
                .Select(z => new ZoneDTO
                {
                    ID = z.ID,
                    Name = z.Name,
                    Description = z.Description,
                    LocationName = z.LocationName,
                    IsActive = z.IsActive
                }).ToList();

            if (activeManagerZones.Any())
            {
                return Ok(activeManagerZones);
            }

            // If no active zones for the manager, return other active zones
            var fallbackZones = await _context.Zones
                .Where(z => z.IsActive)
                .ToListAsync();

            return Ok(fallbackZones);
        }

        [HttpGet("zone/{zoneId}/seats/{date}")]
        public async Task<IActionResult> GetSeatAvailability(int zoneId, DateTime date)
        {
            // Get all seats in the zone
            var seatsInZone = await _context.Seats
                .Where(s => s.ZoneId == zoneId && s.IsActive)
                .ToListAsync();

            if (!seatsInZone.Any())
                return NotFound("No active seats found in the zone.");

            var seatIds = seatsInZone.Select(s => s.ID).ToList();

            // Get reservations for the given date
            var reservations = await _context.Reservations
                .Where(r => seatIds.Contains(r.SeatID) && r.ReservedDate == date.Date)
                .Select(r => r.SeatID)
                .ToListAsync();

            // Build result DTO
            var seatStatusList = seatsInZone.Select(seat => new SeatDTO
            {
                ID = seat.ID,
                Number = seat.Number,
                IsBooked = reservations.Contains(seat.ID)
            }).ToList();

            return Ok(seatStatusList);
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveSeat([FromBody] ReservationDto request)
        {
            // Validate input
            if (request == null || string.IsNullOrEmpty(request.UserADID))
                return BadRequest("Invalid reservation request.");

            // Check if seat exists and is active
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.ID == request.SeatID && s.IsActive);

            if (seat == null)
                return NotFound("Seat not found or inactive.");

            // Check if seat is already reserved on the date
            bool isAlreadyReserved = await _context.Reservations
                .AnyAsync(r => r.SeatID == request.SeatID && r.ReservedDate == request.ReservedDate.Date);

            if (isAlreadyReserved)
                return Conflict("Seat is already reserved for the selected date.");

            // Create the reservation
            var reservation = new Reservation
            {
                UserADID = request.UserADID,
                ReservedByADID = request.ReservedByADID, // could be same as UserADID or null
                SeatID = request.SeatID,
                ReservedDate = request.ReservedDate.Date,
                InsertedOn = DateTime.UtcNow,
                IsSmsSent = false // default
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("bulkreserve")]
        public async Task<IActionResult> BulkReserve([FromBody] BulkReservationDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.RequestedByADID) || request.Reservations == null || !request.Reservations.Any())
                return BadRequest("Invalid request data.");

            // Check the requester user and role
            var requester = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.ADID == request.RequestedByADID);

            if (requester == null)
                return NotFound("Requesting user not found.");

            if (requester.Role == null || !string.Equals(requester.Role.Name, "Lead", StringComparison.OrdinalIgnoreCase))
                return Forbid("Only users with Lead role can perform bulk reservations.");

            // Extract unique seat IDs and dates from the reservations
            var seatIds = request.Reservations.Select(r => r.SeatID).Distinct().ToList();

            // Validate seats exist and are active
            var seats = await _context.Seats
                .Where(s => seatIds.Contains(s.ID) && s.IsActive)
                .ToListAsync();

            if (seats.Count != seatIds.Count)
                return NotFound("One or more seats not found or inactive.");

            // Check for conflicting reservations for the same seat and date
            var conflicts = await _context.Reservations
                .Where(r => seatIds.Contains(r.SeatID))
                .Where(r => request.Reservations.Any(rr => rr.SeatID == r.SeatID && rr.ReservedDate.Date == r.ReservedDate))
                .Select(r => new { r.SeatID, r.ReservedDate })
                .ToListAsync();

            if (conflicts.Any())
            {
                return Conflict(new
                {
                    message = "Some seats are already reserved on the specified dates.",
                    conflicts = conflicts
                });
            }

            // Create reservation entities from DTOs
            var now = DateTime.UtcNow;
            var reservationsToAdd = request.Reservations.Select(r => new Reservation
            {
                UserADID = r.UserADID,
                ReservedByADID = r.ReservedByADID ?? request.RequestedByADID,
                SeatID = r.SeatID,
                ReservedDate = r.ReservedDate.Date,
                InsertedOn = now,
                IsSmsSent = false
            }).ToList();

            _context.Reservations.AddRange(reservationsToAdd);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Bulk reservation completed successfully.",
                reservedCount = reservationsToAdd.Count
            });
        }

        [HttpPut("reservation/{userAdid}/{date}")]
        public async Task<IActionResult> UpdateCheckInOut(string userAdid, DateTime date)
        {
            if (string.IsNullOrEmpty(userAdid))
                return BadRequest("User ADID is required.");

            // Find reservation for user and date
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.UserADID == userAdid && r.ReservedDate == date.Date);

            if (reservation == null)
                return NotFound("Reservation not found for user on specified date.");

            // Get current time in HH:mm format
            var currentTime = DateTime.Now.ToString("HH:mm");

            if (string.IsNullOrEmpty(reservation.CheckInTime))
            {
                // Update check-in time if null
                reservation.CheckInTime = currentTime;
            }
            else
            {
                // Else update check-out time
                reservation.CheckOutTime = currentTime;
            }

            reservation.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(reservation);
        }



    }
}

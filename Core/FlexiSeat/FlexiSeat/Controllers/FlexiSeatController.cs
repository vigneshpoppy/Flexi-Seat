using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FlexiSeatController : ControllerBase
    {
       
        private readonly ILogger<FlexiSeatController> _logger;
        private readonly FlexiSeatDbContext _context;

        public FlexiSeatController(ILogger<FlexiSeatController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }   

        [HttpGet("seats")]
        public async Task<IActionResult> GetAllSeats()
        {
            var seats = await _context.Seats.ToListAsync();
            return Ok(seats);
        }

        [HttpGet("seats/{id}")]
        public async Task<IActionResult> GetSeatById(string id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat == null) return NotFound();
            return Ok(seat);
        }

        [HttpGet("zone/{zone}")]
        public async Task<IActionResult> GetSeatsByZone(string zone)
        {
            var seats = await _context.Seats
                                      .Where(s => s.Zone.ToUpper().Equals(zone.ToUpper()))
                                      .ToListAsync();

            if (!seats.Any())
                return NotFound($"No seats found for zone '{zone}'.");

            return Ok(seats);
        }

        [HttpPost]
        public async Task<IActionResult> AddSeat([FromBody] Seat newSeat)
        {
            if (newSeat == null)
                return BadRequest("Seat data is null.");

            // Optional: Check if a seat with the same SeatNo already exists
            var exists = await _context.Seats.FindAsync(newSeat.SeatNo);
            if (exists != null)
                return Conflict("Seat with the same SeatNo already exists.");

            await _context.Seats.AddAsync(newSeat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSeatById), new { seatNo = newSeat.SeatNo }, newSeat);
        }

        [HttpPut("{seatNo}")]
        public async Task<IActionResult> UpdateSeat(string seatNo, [FromBody] Seat updatedSeat)
        {
            if (seatNo != updatedSeat.SeatNo)
                return BadRequest("SeatNo mismatch.");

            var seat = await _context.Seats.FindAsync(seatNo);
            if (seat == null)
                return NotFound("Seat not found.");

            // Update properties
            seat.LocationName = updatedSeat.LocationName;
            seat.Zone = updatedSeat.Zone;
            seat.ManagerID = updatedSeat.ManagerID;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{seatNo}")]
        public async Task<IActionResult> DeleteSeat(string seatNo)
        {
            var seat = await _context.Seats.FindAsync(seatNo);
            if (seat == null)
                return NotFound("Seat not found.");

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();

            return Ok("Seat deleted successfully.");
        }


        [HttpPost("reservation")]
        public async Task<IActionResult> ReserveSeat([FromBody] ReservationDto dto)
        {
            var reservation = new Reservation
            {
                UserADID = dto.UserADID,
                SeatNo = dto.SeatNo,
                ReserveDate = dto.ReserveDate,
                Status = dto.Status,
                InsertedOn = DateTime.UtcNow,
                UpdatedOn=DateTime.Now,
                
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }
        [HttpPost("reservations/bulk")]
        public async Task<IActionResult> BulkReserve([FromBody] BulkReservationDto dto)
        {
            var position = await _context.Designations
                              .Where(x => x.ADID == dto.RequestedByADID)
                              .Select(x => x.Position)
                              .FirstOrDefaultAsync();

            if (position < 3)  // Assume only Position 3+ can bulk reserve
                return Forbid("Your role does not permit bulk reservation.");

            if (dto.Reservations.Count < 2 || dto.Reservations.Count > 40)
                return BadRequest("You can only reserve for 2 to 40 users.");

            foreach (var res in dto.Reservations)
            {
                _context.Reservations.Add(new Reservation
                {
                    UserADID = res.UserADID,
                    SeatNo = res.SeatNo,
                    ReserveDate = res.ReserveDate,
                    Status = res.Status,
                    InsertedOn = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok("Bulk reservation successful.");
        }
        [HttpGet("report/{id}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }
        [HttpPost("reservation/checkin")]
        public async Task<IActionResult> CheckInByBadge([FromBody] string badgeId)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(e => e.BadgeID == badgeId);
            if (user == null) return NotFound("User not found");

            var reservation = await _context.Reservations
                .Where(r => r.UserADID == user.UserADID && r.ReserveDate == DateTime.Today)
                .FirstOrDefaultAsync();

            if (reservation == null) return NotFound("Reservation not found");

            reservation.CheckInTime = DateTime.UtcNow;
            reservation.Status = 2; // Checked-in
            await _context.SaveChangesAsync();

            return Ok("Check-in successful");
        }

        [HttpPost("reservation/checkout")]
        public async Task<IActionResult> CheckOutByBadge([FromBody] string badgeId)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(e => e.BadgeID == badgeId);
            if (user == null) return NotFound("User not found");

            var reservation = await _context.Reservations
                .Where(r => r.UserADID == user.UserADID && r.ReserveDate == DateTime.Today)
                .FirstOrDefaultAsync();

            if (reservation == null) return NotFound("Reservation not found");

            reservation.CheckOutTime = DateTime.UtcNow;
            reservation.Status = 3; // Checked-out
            await _context.SaveChangesAsync();

            return Ok("Check-out successful");
        }
    }
}

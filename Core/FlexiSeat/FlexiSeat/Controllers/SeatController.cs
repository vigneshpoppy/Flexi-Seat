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
    public class SeatController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly ILogger<SeatController> _logger;
        public SeatController(ILogger<SeatController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("GetAllSeats")]
        public async Task<IActionResult> GetAllSeats()
        {
            var seats = await _context.Seats
                .Include(s => s.Zone)
                .Select(s => new SeatDTO
                {
                    ID = s.ID,
                    Number = s.Number,
                    ZoneId = s.ZoneId,
                    IsActive = s.IsActive,
                    ZoneName = s.Zone.Name
                })
                .ToListAsync();

            return Ok(seats);
        }

        [HttpGet("GetSeatById/{id}")]
        public async Task<IActionResult> GetSeatById(int id)
        {
            var seat = await _context.Seats
                .Include(s => s.Zone)
                .Where(s => s.ID == id)
                .Select(s => new SeatDTO
                {
                    ID = s.ID,
                    Number= s.Number,
                    ZoneId = s.ZoneId,
                    IsActive = s.IsActive,
                    ZoneName = s.Zone.Name
                })
                .FirstOrDefaultAsync();

            if (seat == null)
                return NotFound($"Seat with ID '{id}' not found.");

            return Ok(seat);
        }

        [HttpPost("CreateSeat")]
        public async Task<IActionResult> CreateSeat([FromBody] SeatDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingSeat = await _context.Seats.FirstOrDefaultAsync(r => r.Number == dto.Number && r.IsActive == true && r.ZoneId == dto.ZoneId);

            if (existingSeat != null)
            {
                return Conflict(new { message = "Same seat number already exists for the zone" });
            }

            var seat = new Seat
            {
                Number = dto.Number,
                ZoneId = dto.ZoneId,
                IsActive = dto.IsActive
            };

            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSeatById), new { id = seat.ID }, seat);
        }

        [HttpPut("UpdateSeat/{id}")]
        public async Task<IActionResult> UpdateSeat(int id, [FromBody] SeatDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var seat = await _context.Seats.FindAsync(id);
            if (seat == null)
                return NotFound($"Seat with ID '{id}' not found.");

            var existingSeat = await _context.Seats.FirstOrDefaultAsync(r => r.Number == dto.Number && r.IsActive == true && r.ZoneId == dto.ZoneId && id != r.ID);

            if (existingSeat != null)
            {
                return Conflict(new { message = "Same seat number already exists for the zone" });
            }

            seat.Number = dto.Number;
            seat.ZoneId = dto.ZoneId;
            seat.IsActive = dto.IsActive;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteSeat/{id}")]
        public async Task<IActionResult> DeleteSeat(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat == null)
                return NotFound($"Seat with ID '{id}' not found.");

            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.SeatDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SeatsController : ControllerBase
  {
    private readonly FlexiSeatDbContext _context;
    public SeatsController(FlexiSeatDbContext context)
    {
      _context = context;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateSeat([FromBody] CreateSeatDTO dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var normalizedNumber = dto.Number?.Trim().ToUpper();

      // Check if a seat with the same number already exists
      bool seatExists = await _context.Seats
          .AnyAsync(s => s.Number == normalizedNumber);

      if (seatExists)
        return Conflict(new { message = $"Seat with number '{dto.Number}' already exists." });

      // Map DTO to entity
      var seat = new Seat
      {
        Number = dto.Number,
        ZoneId = dto.ZoneId,
        IsActive = true
      };

      _context.Seats.Add(seat);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(CreateSeat), new { id = seat.ID }, new
      {
        seat.ID,
        seat.Number,
        seat.ZoneId,
        seat.IsActive
      });
    }

    [HttpGet("{seatNumber}")]
    public async Task<IActionResult> GetSeatInfoBySeatId(string seatNumber)
    {
      var seat = await _context.Seats
                .Include(s => s.Zone)
                .FirstOrDefaultAsync(s => s.Number == seatNumber);

      if (seat == null)
        return NotFound(new { message = $"Seat number {seatNumber} not found." });

      var dto = new GetSeatDTO
      {
        ID = seat.ID,
        Number = seat.Number,
        ZoneId = seat.ZoneId,
        ZoneName = seat.Zone?.Name,
        IsActive = seat.IsActive
      };

      return Ok(dto);
    }

    [HttpGet("Zone/{zoneId}")]
    public async Task<IActionResult> GetSeatInfoByZoneId(int zoneId)
    {
      List<GetSeatDTO> getSeatDTOs = new List<GetSeatDTO>();
      var seats = await _context.Seats
    .Where(s => s.ZoneId == zoneId)   // filter
    .Include(s => s.Zone)             // eager‑load the Zone nav property
    .AsNoTracking()                   // ← optional: read‑only, no change tracking
    .ToListAsync();

      if (seats == null)
        return NotFound(new { message = $"Seat not availble for {seats[0].ZoneId} " });

      foreach (var seat in seats)
      {
        var dto = new GetSeatDTO
        {
          ID = seat.ID,
          Number = seat.Number,
          ZoneId = seat.ZoneId,
          ZoneName = seat.Zone?.Name,
          IsActive = seat.IsActive
        };
        getSeatDTOs.Add(dto);
      }

      return Ok(getSeatDTOs);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllSeats()
    {
      var seats = await _context.Seats
                  .Include(s => s.Zone)
                  .ToListAsync();

      var result = seats.Select(seat => new GetSeatDTO
      {
        ID = seat.ID,
        Number = seat.Number,
        ZoneId = seat.ZoneId,
        ZoneName = seat.Zone?.Name,
        IsActive = seat.IsActive
      });

      return Ok(result);
    }

    [HttpPatch("Update/{id}")]
    public async Task<IActionResult> UpdateSeatById(int id, [FromBody] UpdateSeatDTO dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var seat = await _context.Seats.FindAsync(id);
      if (seat == null)
        return NotFound(new { message = $"Seat with ID {id} not found." });

      if (!string.IsNullOrWhiteSpace(dto.Number))
      {
        var normalizedNumber = dto.Number?.Trim().ToUpper();

        bool numberExists = await _context.Seats
            .AnyAsync(s => s.Number == normalizedNumber && s.ID != id);

        if (numberExists)
          return Conflict(new { message = $"Seat number '{dto.Number?.Trim().ToUpper()}' already exists." });

        seat.Number = normalizedNumber;
      }

      // Update ZoneId if provided and different
      if (dto.ZoneId != 0 && dto.ZoneId != seat.ZoneId)
      {
        seat.ZoneId = dto.ZoneId;
      }

      if (dto.IsActive != null && (seat.IsActive != dto.IsActive))
        seat.IsActive = !seat.IsActive;

      await _context.SaveChangesAsync();

      return Ok(new { message = $"Seat with ID {id} updated successfully." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSeatById(int id)
    {
      var seat = await _context.Seats.FindAsync(id);

      if (seat == null)
        return NotFound(new { message = $"Seat with ID {id} not found." });

      _context.Seats.Remove(seat);
      await _context.SaveChangesAsync();

      return Ok(new { message = $"Seat with ID {id} deleted successfully." });
    }
  }
}

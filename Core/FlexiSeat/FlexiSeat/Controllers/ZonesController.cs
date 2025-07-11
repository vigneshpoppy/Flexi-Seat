using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.ZoneDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZonesController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        public ZonesController(FlexiSeatDbContext context)
        {
          _context = context;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateZone([FromBody] CreateZoneDTO dto)
        {
            if (!ModelState.IsValid)
              return BadRequest(ModelState);

            var normalizedName = dto.Name?.Trim().ToUpper();

            // Check if Zone with same name already exists
            bool exists = await _context.Zones.AnyAsync(z => z.Name == normalizedName);
            if (exists)
              return Conflict(new { message = $"Zone with name '{dto.Name}' already exists." });

            // Map DTO to Entity
            var zone = new Zone
            {
              Name = dto.Name,
              Description = dto.Description,
              IsActive = true,
              ManagerADID = dto.ManagerADID
            };

            _context.Zones.Add(zone);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetZoneById), new { id = zone.ID }, new
            {
              zone.ID,
              zone.Name,
              zone.Description,
              zone.IsActive,
              zone.ManagerADID
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetZoneById(int id)
        {
            var zone = await _context.Zones
              .Include(z => z.Manager)
              .FirstOrDefaultAsync(z => z.ID == id);

            if (zone == null)
              return NotFound(new { message = $"Zone with ID {id} not found." });

            var dto = new GetZoneDTO
            {
              ID = id,
              Name = zone.Name,
              Description = zone.Description,
              IsActive = zone.IsActive,
              ManagerADID = zone.ManagerADID,
              ManagerName = zone.Manager?.Name
            };

            return Ok(dto);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllZones()
        {
              var zones = await _context.Zones
                .Include(z => z.Manager)
                .ToListAsync();

              var result = zones.Select(zone => new GetZoneDTO
              {
                ID = zone.ID,
                Name = zone.Name,
                Description = zone.Description,
                IsActive = zone.IsActive,
                ManagerADID = zone.ManagerADID,
                ManagerName = zone.Manager?.Name
              });

              return Ok(result);
        }

        [HttpPatch("Update/{id}")]
        public async Task<IActionResult> UpdateZoneById(int id, [FromBody] UpdateZoneDTO dto)
        {
            if (!ModelState.IsValid)
              return BadRequest(ModelState);

            var zone = await _context.Zones.FirstOrDefaultAsync(z => z.ID == id);
            if (zone == null)
              return NotFound(new { message = $"Zone with ID {id} not found." });

            string? normalizedName = dto.Name?.Trim().ToUpper();

            // Check for unique name if it's being updated
            if (!string.IsNullOrWhiteSpace(normalizedName) && normalizedName != zone.Name)
            {
              bool nameExists = await _context.Zones.AnyAsync(z => z.Name == normalizedName && z.ID != id);
              if (nameExists)
                return Conflict(new { message = $"Zone with name '{dto.Name?.Trim().ToUpper()}' already exists." });

              zone.Name = normalizedName;
            }

            // Update optional fields if provided
            if (!string.IsNullOrWhiteSpace(dto.Description))
              zone.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.ManagerADID))
              zone.ManagerADID = dto.ManagerADID?.Trim().ToUpper();

            // IsActive is non-nullable, so always update
            if(dto.IsActive != null && (zone.IsActive != dto.IsActive))
              zone.IsActive = !zone.IsActive;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Zone with ID {id} updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteZoneById(int id)
        {
            var zone = await _context.Zones.FindAsync(id);
            if (zone == null)
              return NotFound(new { message = $"Zone with ID {id} not found." });

            _context.Zones.Remove(zone);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Zone with ID {id} deleted successfully." });
        }
    }
}

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
    public class ZoneController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly ILogger<ZoneController> _logger;
        public ZoneController(ILogger<ZoneController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("GetAllZones")]
        public async Task<IActionResult> GetAllZones()
        {
            var zones = await _context.Zones
                .Include(z => z.Manager)
                .Select(z => new ZoneDTO
                {
                    ID = z.ID,
                    Name = z.Name,
                    Description = z.Description,
                    LocationName = z.LocationName,
                    IsActive = z.IsActive,
                    ManagerADID = z.ManagerADID,
                    ManagerName = z.Manager != null ? z.Manager.Name : null
                })
                .ToListAsync();

            return Ok(zones);
        }

        [HttpGet("GetZoneById/{id}")]
        public async Task<IActionResult> GetZoneById(int id)
        {
            var zone = await _context.Zones
                .Include(z => z.Manager)
                .Where(z => z.ID == id)
                .Select(z => new ZoneDTO
                {
                    ID = z.ID,
                    Name = z.Name,
                    Description = z.Description,
                    LocationName = z.LocationName,
                    IsActive = z.IsActive,
                    ManagerADID = z.ManagerADID,
                    ManagerName = z.Manager != null ? z.Manager.Name : null
                })
                .FirstOrDefaultAsync();

            if (zone == null)
                return NotFound($"Zone with ID '{id}' not found.");

            return Ok(zone);
        }

        [HttpPost("CreateZone")]
        public async Task<IActionResult> CreateZone([FromBody] ZoneDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingZone = await _context.Zones.FirstOrDefaultAsync(r => r.Name == dto.Name && r.IsActive == true && r.LocationName == dto.LocationName);

            if(existingZone != null)
            {
                return Conflict(new { message = dto.Name.ToUpper() + " zone already exists for the location" });
            }

            var zone = new Zone
            {
                Name = dto.Name,
                Description = dto.Description,
                LocationName = dto.LocationName,
                IsActive = dto.IsActive,
                ManagerADID = dto.ManagerADID
            };

            _context.Zones.Add(zone);
            await _context.SaveChangesAsync();

            // Return 201 Created with route to newly created resource
            return CreatedAtAction(nameof(GetZoneById), new { id = zone.ID }, zone);
        }

        [HttpPut("UpdateZoneById/{id}")]
        public async Task<IActionResult> UpdateZoneById(int id, [FromBody] ZoneDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var zone = await _context.Zones.FindAsync(id);
            if (zone == null)
                return NotFound($"Zone with ID '{id}' not found.");

            var zoneNameDuplicate = await _context.Zones
                      .FirstOrDefaultAsync(r => r.Name == dto.Name && r.LocationName != dto.LocationName && id != r.ID);

            if (zoneNameDuplicate != null)
            {
                return Conflict(new { message = "Same zone name already exists in the location" });
            }

            zone.Name = dto.Name;
            zone.Description = dto.Description;
            zone.LocationName = dto.LocationName;
            zone.IsActive = dto.IsActive;
            zone.ManagerADID = dto.ManagerADID;

            _context.Zones.Update(zone);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteZoneById/{id}")]
        public async Task<IActionResult> DeleteZoneById(int id)
        {
            var zone = await _context.Zones.FindAsync(id);
            if (zone == null)
                return NotFound($"Zone with ID '{id}' not found.");

            _context.Zones.Remove(zone);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO;
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
      //Validation
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      //Zone with same name should not exist
      var existingZone = await _context.Zones.FirstOrDefaultAsync(r => r.Name == dto.Name && r.IsActive == true);

      if (existingZone != null)
      {
        return Conflict(new { message = dto.Name.ToUpper() + " zone already exists for the location" });
      }

      //Update zone to DB
      var zone = new Zone
      {
        Name = dto.Name,
        Description = dto.Description,
        LocationName = dto.LocationName,
        IsActive = true,
        ManagerADID = dto.ManagerADID
      };

      _context.Zones.Add(zone);
      await _context.SaveChangesAsync();

      //Send reponse back
      return CreatedAtAction(nameof(GetZoneById), new { id = zone.ID }, zone);
    }

    [HttpGet("")]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetZoneById(int id)
    {
      var zone = await _context.Zones
          .Include(z => z.Manager)
          .Where(z => z.ID == id)
          .Select(z => new GetZoneDTO
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

    [HttpPatch("Update/{id}")]
    public async Task<IActionResult> UpdateZoneById(int id, [FromBody] UpdateZoneDTO dto)
    {
      //Validate
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      //Check if zone exist
      var zone = await _context.Zones.FindAsync(id);
      if (zone == null)
        return NotFound($"Zone with ID '{id}' not found.");

      //Check if zone name is not duplicate
      var zoneNameDuplicate = await _context.Zones
                .FirstOrDefaultAsync(r => r.Name == dto.Name && id != r.ID);

      if (zoneNameDuplicate != null)
      {
        return Conflict(new { message = "Same zone name already exists" });
      }

      //Update to db
      zone.Name = dto.Name;
      zone.Description = dto.Description;
      zone.LocationName = dto.LocationName;
      zone.IsActive = dto.IsActive;
      zone.ManagerADID = dto.ManagerADID;

      _context.Zones.Update(zone);
      await _context.SaveChangesAsync();

      //Send reponse back
      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteZoneById(int id)
    {
      //Check if zone exist
      var zone = await _context.Zones.FindAsync(id);
      if (zone == null)
        return NotFound($"Zone with ID '{id}' not found.");

      //Delete from DB
      _context.Zones.Remove(zone);
      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
}

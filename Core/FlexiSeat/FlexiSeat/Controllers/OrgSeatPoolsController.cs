using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.OrgSeatPoolDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OrgSeatPoolsController : ControllerBase
  {
    private readonly FlexiSeatDbContext _context;
    public OrgSeatPoolsController(FlexiSeatDbContext context)
    {
      _context = context;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateOrgSeatPool([FromBody] CreateOrgSeatPoolDTO dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Normalize input
      var normalizedManagerAdid = dto.ManagerADID?.Trim().ToUpper();

      // Check if Zone exists
      var zoneExists = await _context.Zones.AnyAsync(z => z.ID == dto.ZoneId);
      if (!zoneExists)
        return NotFound($"Zone with ID '{dto.ZoneId}' not found.");

      // Optional: Check if Manager exists (only if ADID is provided)
      if (!string.IsNullOrEmpty(normalizedManagerAdid))
      {
        var managerExists = await _context.Users.AnyAsync(u => u.ADID == normalizedManagerAdid);
        if (!managerExists)
          return NotFound($"Manager with ADID '{normalizedManagerAdid}' not found.");
      }

      // Optional: Check for duplicate OrgSeatPool (e.g., same ZoneId & ManagerADID)
      var existing = await _context.OrgSeatPools.AnyAsync(p =>
          p.ZoneId == dto.ZoneId &&
          (string.IsNullOrEmpty(normalizedManagerAdid) ? p.ManagerADID == null : p.ManagerADID == normalizedManagerAdid));

      if (existing)
        return Conflict("An OrgSeatPool entry for this zone and manager already exists.");

      // Create new OrgSeatPool entity
      var newPool = new OrgSeatPool
      {
        ZoneId = dto.ZoneId,
        ManagerADID = normalizedManagerAdid,
        SeatsAllotted = dto.SeatsAllotted
      };

      _context.OrgSeatPools.Add(newPool);
      await _context.SaveChangesAsync();

      return Ok(new { message = "OrgSeatPool created successfully.", newPool.ID });
    }

    [HttpGet("{managerADID}")]
    public async Task<IActionResult> GetOrgSeatPoolById(string managerADID)
    {
      var orgSeatPools = await _context.OrgSeatPools.Where(s => s.ManagerADID == managerADID && s.Zone.IsActive==true)
          .Include(p => p.Zone).AsNoTracking().ToListAsync();
      List<GetOrgSeatPoolDTO> lstGetOrgSeatPoolDTO = new List<GetOrgSeatPoolDTO>();
      if (orgSeatPools == null)
        return NotFound($"OrgSeatPool with Manager ID '{managerADID}' not found.");
      foreach (var orgSeatPool in orgSeatPools)
      {
        var result = new GetOrgSeatPoolDTO
        {
          ID = orgSeatPool.ID,
          ZoneId = orgSeatPool.ZoneId,
          ZoneName = orgSeatPool.Zone?.Name ?? string.Empty,
          ManagerADID = orgSeatPool.ManagerADID,
          ManagerName = orgSeatPool.Manager?.Name,
          SeatsAllotted = orgSeatPool.SeatsAllotted
        };
        lstGetOrgSeatPoolDTO.Add(result);
      }
      return Ok(lstGetOrgSeatPoolDTO);
    }

    [HttpGet("All")]
    public async Task<IActionResult> GetAllOrgSeatPools()
    {
      var pools = await _context.OrgSeatPools
          .Include(p => p.Zone)
          .ToListAsync();

      if (pools == null || !pools.Any())
        return NoContent(); // 204 No Content

      var result = pools.Select(p => new GetOrgSeatPoolDTO
      {
        ID = p.ID,
        ZoneId = p.ZoneId,
        ZoneName = p.Zone?.Name ?? string.Empty,
        ManagerADID = p.ManagerADID,
        ManagerName = p.Manager?.Name,
        SeatsAllotted = p.SeatsAllotted
      }).ToList();

      return Ok(result);
    }

    [HttpPatch("Update/{id}")]
    public async Task<IActionResult> UpdateOrgSeatPoolById(int id, [FromBody] UpdateOrgSeatPoolDTO dto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Get the existing OrgSeatPool entry
      var orgSeatPool = await _context.OrgSeatPools.FindAsync(id);
      if (orgSeatPool == null)
        return NotFound($"OrgSeatPool with ID {id} not found.");

      // Check for duplicate ZoneId (if changed)
      if (dto.ZoneId != 0 && dto.ZoneId != orgSeatPool.ZoneId)
      {
        var duplicateZone = await _context.OrgSeatPools
            .AnyAsync(p => p.ZoneId == dto.ZoneId && p.ID != id);
        if (duplicateZone)
          return Conflict($"An OrgSeatPool already exists for Zone ID {dto.ZoneId}.");

        orgSeatPool.ZoneId = dto.ZoneId;
      }

      // Update ManagerADID if provided or remove if ShowRemoveManagerOption is true
      if (dto.ShowRemoveManagerOption == true)
      {
        orgSeatPool.ManagerADID = null;
      }
      else if (!string.IsNullOrWhiteSpace(dto.ManagerADID))
      {
        // Check if Manager exists
        var managerExists = await _context.Users.AnyAsync(u => u.ADID == dto.ManagerADID.Trim().ToUpper());
        if (!managerExists)
          return NotFound($"Manager with ADID '{dto.ManagerADID}' not found.");

        orgSeatPool.ManagerADID = dto.ManagerADID.Trim().ToUpper();
      }

      // Update SeatsAllotted if provided
      if (dto.SeatsAllotted.HasValue)
      {
        if (dto.SeatsAllotted.Value < 0)
          return BadRequest("SeatsAllotted must be a positive value.");

        orgSeatPool.SeatsAllotted = dto.SeatsAllotted.Value;
      }

      await _context.SaveChangesAsync();
      return Ok(new { message = "OrgSeatPool updated successfully." });
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteOrgSeatPoolById(int id)
    {
      var orgSeatPool = await _context.OrgSeatPools.FindAsync(id);

      if (orgSeatPool == null)
        return NotFound($"OrgSeatPool with ID {id} not found.");

      _context.OrgSeatPools.Remove(orgSeatPool);
      await _context.SaveChangesAsync();

      return Ok(new { message = $"OrgSeatPool with ID {id} deleted successfully." });
    }
  }
}

using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO;
using FlexiSeat.DTO.AppRoleDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class RolesController : ControllerBase
  {
    private readonly FlexiSeatDbContext _context;
    public RolesController(FlexiSeatDbContext context)
    {
      _context = context;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateAppRoleDTO dto)
    {
      //Validate
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      //Role should not exist
      var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.Name && r.IsActive == true);
      if (role != null)
      {
        return Conflict(new { message = dto.Name.ToUpper() + " role already exists." });
      }

      //Update role to DB
      var newRole = new AppRole
      {
        Name = dto.Name.ToUpper(),
        Description = dto.Description,
        IsActive = true
      };

      _context.Roles.Add(newRole);
      await _context.SaveChangesAsync();

      //Send reponse back
      return CreatedAtAction(nameof(GetRoleById), new { id = newRole.ID }, newRole);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllRoles()
    {
      var roles = await _context.Roles
          .Select(r => new GetAppRoleDTO
          {
            ID = r.ID,
            Name = r.Name.ToUpper(),
            Description = r.Description,
            IsActive = r.IsActive
          })
          .ToListAsync();

      return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById(int id)
    {
      var role = await _context.Roles
          .Where(r => r.ID == id)
          .Select(r => new GetAppRoleDTO
          {
            ID = r.ID,
            Name = r.Name.ToUpper(),
            Description = r.Description,
            IsActive = r.IsActive
          })
          .FirstOrDefaultAsync();

      if (role == null)
        return NotFound($"Role with ID '{id}' not found.");

      return Ok(role);
    }

    [HttpPatch("Update/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateAppRoleDTO dto)
    {
      //Validate
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      //Check if role exist
      var role = await _context.Roles.FindAsync(id);
      if (role == null)
        return NotFound($"Role with ID '{id}' not found.");

      //Check if role name is not duplicate
      var roleNameDuplicate = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == dto.Name && id != r.ID);

      if (roleNameDuplicate != null)
      {
        return Conflict(new { message = "Same role name already exists" });
      }

      //Update to DB
      role.Name = dto.Name.ToUpper();
      role.Description = dto.Description;
      role.IsActive = dto.IsActive;

      _context.Roles.Update(role);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
      //Check if role exist
      var role = await _context.Roles.FindAsync(id);
      if (role == null)
        return NotFound($"Role with ID '{id}' not found.");

      //Check if any users are assigned to this role
      var hasUsers = await _context.Users.AnyAsync(u => u.RoleId == id);
      if (hasUsers)
        return Conflict("Cannot delete this role because it is assigned to one or more users.");

      //Delete from DB
      _context.Roles.Remove(role);
      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
}

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
    public class RoleController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly ILogger<FlexiSeatController> _logger;
        public RoleController(ILogger<FlexiSeatController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Roles
                .Select(r => new AppRoleDTO
                {
                    ID = r.ID,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpGet("GetRoleById/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _context.Roles
                .Where(r => r.ID == id)
                .Select(r => new AppRoleDTO
                {
                    ID = r.ID,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive
                })
                .FirstOrDefaultAsync();

            if (role == null)
                return NotFound($"Role with ID '{id}' not found.");

            return Ok(role);
        }
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] AppRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = new AppRole
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Return 201 Created with route to GetRoleById
            return CreatedAtAction(nameof(GetRoleById), new { id = role.ID }, role);
        }

        [HttpPut("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] AppRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound($"Role with ID '{id}' not found.");

            role.Name = dto.Name;
            role.Description = dto.Description;
            role.IsActive = dto.IsActive;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content on successful update
        }

        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound($"Role with ID '{id}' not found.");

            // Optional: Check if any users are assigned to this role
            var hasUsers = await _context.Users.AnyAsync(u => u.RoleId == id);
            if (hasUsers)
                return Conflict("Cannot delete this role because it is assigned to one or more users.");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

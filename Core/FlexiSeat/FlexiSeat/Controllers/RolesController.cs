using System.Data;
using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.RoleDTOs;
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
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO dto)
        {
            // Step 1: Validate the input model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Step 2: Normalize input for comparison
            var normalizedName = dto.Name?.Trim().ToUpper();

            // Step 3: Check if the name already exists in the DB
            bool exists = await _context.Roles
                .AnyAsync(r => r.Name == normalizedName);

            if (exists)
            {
                return Conflict(new { message = $"A role with name '{dto.Name?.Trim()}' already exists." });
            }

            // Step 4: Create and save the new role
            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true// the setter will normalize (Trim + ToUpper)
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Step 5: Return created result
            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, new { role.Id, role.Name });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }

            var dto = new GetRoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive
            };

            return Ok(dto);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Roles
                .Select(r => new GetRoleDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpPatch("Update/{id}")]
        public async Task<IActionResult> UpdateRoleById(int id, [FromBody] UpdateRoleDTO dto)
        {
            // 1. Validate model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2. Find existing role by id
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }

            // 3. Normalize input name
            var normalizedName = dto.Name?.Trim().ToUpper();

            // 4. Check if another role with this name exists (excluding current role)
            bool nameExists = await _context.Roles
                .AnyAsync(r => r.Name == normalizedName && r.Id != id);

            if (nameExists)
            {
                return Conflict(new { message = $"A role with name '{dto.Name}' already exists." });
            }

            // 5. Update and save
            role.Name = dto.Name;  // setter will normalize
                                   // Update optional fields if provided
            if (!string.IsNullOrWhiteSpace(dto.Description))
              role.Description = dto.Description;

            // IsActive is non-nullable, so always update
            if (dto.IsActive != null && (role.IsActive != dto.IsActive))
              role.IsActive = !role.IsActive;

            await _context.SaveChangesAsync();

            // 6. Return success
            return Ok(new { role.Id, role.Name });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleById(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Role deleted successfully." });
        }

    }
}

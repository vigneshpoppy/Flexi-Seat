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
    public class UserController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                        .Include(u => u.Role)
                        .Include(u => u.Lead)
                        .Include(u => u.Manager)
                        .Select(u => new UserDTO
                        {
                            ADID = u.ADID,
                            Name = u.Name,
                            Designation = u.Designation,
                            BadgeID = u.BadgeID,
                            RoleId = u.RoleId,
                            RoleName = u.Role != null ? u.Role.Name : null,
                            LeadADID = u.LeadADID,
                            LeadName = u.Lead != null ? u.Lead.Name : null,
                            ManagerADID = u.ManagerADID,
                            ManagerName = u.Manager != null ? u.Manager.Name : null
                        })
                        .ToListAsync();

            return Ok(users);
        }

        [HttpGet("GetUserById/{adid}")]
        public async Task<IActionResult> GetUserById(string adid)
        {
            var user = await _context.Users
                        .Include(u => u.Role)
                        .Include(u => u.Lead)
                        .Include(u => u.Manager)
                        .Where(u => u.ADID == adid)
                        .Select(u => new UserDTO
                        {
                            ADID = u.ADID,
                            Name = u.Name,
                            Designation = u.Designation,
                            BadgeID = u.BadgeID,
                            RoleId = u.RoleId,
                            RoleName = u.Role != null ? u.Role.Name : null,
                            LeadADID = u.LeadADID,
                            LeadName = u.Lead != null ? u.Lead.Name : null,
                            ManagerADID = u.ManagerADID,
                            ManagerName = u.Manager != null ? u.Manager.Name : null
                        })
                        .FirstOrDefaultAsync();

            if (user == null)
                return NotFound($"User with ADID '{adid}' not found.");

            return Ok(user);
        }

        [HttpGet("GetUserByBadgeId/{badgeId}")]
        public async Task<IActionResult> GetUserByBadgeId(string badgeId)
        {
            var user = await _context.Users
                        .Include(u => u.Role)
                        .Include(u => u.Lead)
                        .Include(u => u.Manager)
                        .Where(u => u.BadgeID == badgeId)
                        .Select(u => new UserDTO
                        {
                            ADID = u.ADID,
                            Name = u.Name,
                            Designation = u.Designation,
                            BadgeID = u.BadgeID,
                            RoleId = u.RoleId,
                            RoleName = u.Role != null ? u.Role.Name : null,
                            LeadADID = u.LeadADID,
                            LeadName = u.Lead != null ? u.Lead.Name : null,
                            ManagerADID = u.ManagerADID,
                            ManagerName = u.Manager != null ? u.Manager.Name : null
                        })
                        .FirstOrDefaultAsync();

            if (user == null)
                return NotFound($"User with BadgeID '{badgeId}' not found.");

            return Ok(user);
        }

        [HttpGet("GetUsersByLeadId/{leadAdid}")]
        public async Task<IActionResult> GetUsersByLeadId(string leadAdid)
        {
            var users = await _context.Users
                .Where(u => u.LeadADID == leadAdid)
                .Include(u => u.Role)
                .Include(u => u.Lead)
                .Include(u => u.Manager)
                .Select(u => new UserDTO
                {
                    ADID = u.ADID,
                    Name = u.Name,
                    Designation = u.Designation,
                    BadgeID = u.BadgeID,
                    RoleId = u.RoleId,
                    RoleName = u.Role != null ? u.Role.Name : null,
                    LeadADID = u.LeadADID,
                    LeadName = u.Lead != null ? u.Lead.Name : null,
                    ManagerADID = u.ManagerADID,
                    ManagerName = u.Manager != null ? u.Manager.Name : null
                })
                .ToListAsync();

            if (users == null || users.Count == 0)
                return NotFound($"No users found reporting to Lead ADID '{leadAdid}'.");

            return Ok(users);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicate ADID
            if (await _context.Users.AnyAsync(u => u.ADID == dto.ADID))
                return Conflict($"A user with ADID '{dto.ADID}' already exists.");

            var badgeIdDuplicate = await _context.Users
                      .FirstOrDefaultAsync(r => r.BadgeID == dto.BadgeID);

            if(badgeIdDuplicate != null)
            {
                return Conflict(new { message = "A user with this badgeid already exists." });
            }

            var user = new User
            {
                ADID = dto.ADID,
                Name = dto.Name,
                Designation = dto.Designation,
                BadgeID = dto.BadgeID,
                RoleId = dto.RoleId,
                LeadADID = dto.LeadADID,
                ManagerADID = dto.ManagerADID
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { adid = user.ADID }, user);
        }

        [HttpPut("UpdateUser/{adid}")]
        public async Task<IActionResult> UpdateUser(string adid, [FromBody] UserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(adid);
            if (user == null)
                return NotFound($"User with ADID '{adid}' not found.");

            var badgeIdDuplicate = await _context.Users
                      .FirstOrDefaultAsync(r => r.BadgeID == dto.BadgeID && r.ADID != adid);

            if (badgeIdDuplicate != null)
            {
                return Conflict(new { message = "A user with this badgeid already exists." });
            }

            // Update fields
            user.Name = dto.Name;
            user.Designation = dto.Designation;
            user.BadgeID = dto.BadgeID;
            user.RoleId = dto.RoleId;
            user.LeadADID = dto.LeadADID;
            user.ManagerADID = dto.ManagerADID;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteUser/{adid}")]
        public async Task<IActionResult> DeleteUser(string adid)
        {
            var user = await _context.Users.FindAsync(adid);
            if (user == null)
                return NotFound($"User with ADID '{adid}' not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

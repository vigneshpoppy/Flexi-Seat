using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.UserDTOs;
using FlexiSeat.Helpers;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly IEmailService _emailService;
        public UsersController(FlexiSeatDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            // 1. Validate the DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2. Normalize input for comparison
            var normalizedADID = dto.ADID?.Trim().ToUpper();
            var normalizedBadgeId = dto.BadgeId?.Trim().ToUpper();

            // 3. Check if a user with the same name or badge ID exists
            bool userExists = await _context.Users.AnyAsync(u => u.ADID == normalizedADID);
            if (userExists)
                return Conflict(new { message = $"A user with the ADID '{dto.ADID}' already exists." });

            bool badgeExists = await _context.Users.AnyAsync(u => u.BadgeId == normalizedBadgeId);
            if (badgeExists)
                return Conflict(new { message = $"A user with the badge ID '{dto.BadgeId}' already exists." });

            // 4. Map DTO to entity
            var user = new User
            {
                ADID = dto.ADID,
                Name = dto.Name,
                Designation = dto.Designation,
                BadgeId = dto.BadgeId,
                RoleId = dto.RoleId,
                LeadADID = dto.LeadADID,
                ManagerADID = dto.ManagerADID
            };

            // 5. Save to DB
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            string tempPassword = PasswordHelper.Generate();
            string hashedPassword = PasswordHelper.HashPassword(tempPassword);

            /*Need to remove - writing pwd to file logic - only for test purpose*/
            var logFilePath = "Log.txt";

            using (var writer = new StreamWriter(logFilePath, append: true))
            {
              writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | ADID: {normalizedADID} | Password: {tempPassword}");
            }

            var userLogin = new UserLogin
            {
              ADID = normalizedADID,
              PasswordHash = hashedPassword
            };

            _context.UserLogins.Add(userLogin);
            await _context.SaveChangesAsync();

        /*Need to enable once email server creds are avlbl
            var toEmail = userLogin.ADID + "@yourdomain.com";

            await _emailService.SendEmailAsync(toEmail, "UPS Flexiseat - Temporary Password", $"Your new temporary password is: {tempPassword}");
          */

      // 6. Return success
      return CreatedAtAction(nameof(GetUserByADID), new { adid = user.ADID }, new
              {
                user.ADID,
                user.Name,
                user.Designation,
                user.BadgeId,
                user.RoleId,
                user.LeadADID,
                user.ManagerADID
              }
            );
        }

        [HttpGet("{adid}")]
        public async Task<IActionResult> GetUserByADID(string adid)
        {
            if (string.IsNullOrWhiteSpace(adid))
                return BadRequest(new { message = "ADID is required." });

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Lead)
                .Include(u => u.Manager)
                .FirstOrDefaultAsync(u => u.ADID == adid.Trim().ToUpper());

            if (user == null)
                return NotFound(new { message = $"User with ADID '{adid.Trim().ToUpper()}' not found." });

            var dto = new GetUserDTO
            {
                ADID = user.ADID,
                Name = user.Name,
                Designation = user.Designation,
                BadgeId = user.BadgeId,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
                LeadADID = user.LeadADID,
                LeadName = user.Lead?.Name,
                ManagerADID = user.ManagerADID,
                ManagerName = user.Manager?.Name
            };

            return Ok(dto);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                        .Include(u => u.Role)
                        .Include(u => u.Lead)
                        .Include(u => u.Manager)
                        .ToListAsync();

            var result = users.Select(user => new GetUserDTO
            {
                ADID = user.ADID,
                Name = user.Name,
                Designation = user.Designation,
                BadgeId = user.BadgeId,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name,
                LeadADID = user.LeadADID,
                LeadName = user.Lead?.Name,
                ManagerADID = user.ManagerADID,
                ManagerName = user.Manager?.Name
            });

            return Ok(result);
        }

        [HttpPatch("Update/{adid}")]
        public async Task<IActionResult> UpdateUserByADID(string adid, [FromBody] UpdateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(adid))
                return BadRequest(new { message = "ADID is required." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ADID == adid.Trim().ToUpper());
            if (user == null)
                return NotFound(new { message = $"User with ADID '{adid}' not found." });

            // Normalize input
            string? normalizedBadgeId = dto.BadgeId?.Trim().ToUpper();

            // Check for unique BadgeId (if changed)
            if (!string.IsNullOrWhiteSpace(normalizedBadgeId) && normalizedBadgeId != user.BadgeId)
            {
                bool badgeExists = await _context.Users.AnyAsync(u => u.BadgeId == normalizedBadgeId && u.ADID != user.ADID);
                if (badgeExists)
                    return Conflict(new { message = $"Another user with the badge ID '{dto.BadgeId}' already exists." });

                user.BadgeId = normalizedBadgeId;
            }

            // Conditionally update other fields
            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name?.Trim().ToUpper();

            if (!string.IsNullOrWhiteSpace(dto.Designation))
                user.Designation = dto.Designation?.Trim().ToUpper();

            if (dto.RoleId != 0 && dto.RoleId != user.RoleId)
                user.RoleId = dto.RoleId;

            if (!string.IsNullOrWhiteSpace(dto.LeadADID))
                user.LeadADID = dto.LeadADID?.Trim().ToUpper();

            if (!string.IsNullOrWhiteSpace(dto.ManagerADID))
                user.ManagerADID = dto.ManagerADID?.Trim().ToUpper();

            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully.", user.ADID });
        }

        [HttpDelete("{adid}")]
        public async Task<IActionResult> DeleteUser(string adid)
        {
            if (string.IsNullOrWhiteSpace(adid))
                return BadRequest(new { message = "ADID is required." });

            var normalizedAdid = adid.Trim().ToUpper();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ADID == normalizedAdid);
            if (user == null)
                return NotFound(new { message = $"User with ADID '{adid}' not found." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User with ADID '{adid}' deleted successfully." });
        }
    }
}

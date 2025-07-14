using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.UserDTOs;
using FlexiSeat.Helpers;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
    [Authorize]
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
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreateUsers([FromBody] List<CreateUserDTO> dtos)
    {
      if (dtos is null || dtos.Count == 0)
        return BadRequest("Request body must have at least one user.");

      // 1️⃣ ModelState check for each DTO
      foreach (var dto in dtos)
      {
        TryValidateModel(dto);
      }
      if (!ModelState.IsValid)
        return ValidationProblem(ModelState);

      // 2️⃣ Normalise & detect duplicates inside the payload itself
      var normalized = dtos.Select(dto => new
      {
        Dto = dto,
        AdidNorm = dto.ADID.Trim().ToUpperInvariant(),
        BadgeNorm = dto.BadgeId.Trim().ToUpperInvariant()
      }).ToList();

      var dupAdids = normalized.GroupBy(x => x.AdidNorm)
                                 .Where(g => g.Count() > 1)
                                 .Select(g => g.Key)
                                 .ToHashSet();

      var dupBadges = normalized.GroupBy(x => x.BadgeNorm)
                                 .Where(g => g.Count() > 1)
                                 .Select(g => g.Key)
                                 .ToHashSet();

      if (dupAdids.Any() || dupBadges.Any())
      {
        return Conflict(new
        {
          message = "Duplicate ADID or BadgeId inside the payload.",
          dupAdids,
          dupBadges
        });
      }

      // 3️⃣ Fetch existing ADIDs / BadgeIds once
      var adidSet = normalized.Select(x => x.AdidNorm).ToList();
      var badgeSet = normalized.Select(x => x.BadgeNorm).ToList();

      var existing = await _context.Users
          .Where(u => adidSet.Contains(u.ADID) || badgeSet.Contains(u.BadgeId))
          .Select(u => new { u.ADID, u.BadgeId })
          .ToListAsync();

      var existingAdids = existing.Select(e => e.ADID).ToHashSet(StringComparer.OrdinalIgnoreCase);
      var existingBadges = existing.Select(e => e.BadgeId).ToHashSet(StringComparer.OrdinalIgnoreCase);

      // 4️⃣ Separate insertable vs. skipped
      var toInsert = new List<(User user, UserLogin login, string PlainPassword)>();
      var skipped = new List<object>();

      foreach (var x in normalized)
      {
        if (existingAdids.Contains(x.AdidNorm))
        {
          skipped.Add(new { x.Dto.ADID, Reason = "ADID already exists" });
          continue;
        }
        if (existingBadges.Contains(x.BadgeNorm))
        {
          skipped.Add(new { x.Dto.ADID, Reason = "BadgeId already exists" });
          continue;
        }

        // Map DTO ➜ entity
        var user = new User
        {
          ADID = x.AdidNorm,
          Name = x.Dto.Name,
          Designation = x.Dto.Designation,
          BadgeId = x.BadgeNorm,
          RoleId = x.Dto.RoleId,
          LeadADID = x.Dto.LeadADID?.Trim().ToUpperInvariant(),
          ManagerADID = x.Dto.ManagerADID?.Trim().ToUpperInvariant()
        };

        // Temp password & login
        string plainPwd = PasswordHelper.Generate();
        string hashPwd = PasswordHelper.HashPassword(plainPwd);

        var login = new UserLogin
        {
          ADID = x.AdidNorm,
          PasswordHash = hashPwd
        };

        toInsert.Add((user, login, plainPwd));
      }

      if (toInsert.Count == 0)
        return Conflict(new { message = "No users inserted; all conflicted.", skipped });

      // 5️⃣ Transactional insert
      using var trx = await _context.Database.BeginTransactionAsync();
      try
      {
        _context.Users.AddRange(toInsert.Select(t => t.user));
        _context.UserLogins.AddRange(toInsert.Select(t => t.login));
        await _context.SaveChangesAsync();
        await trx.CommitAsync();
      }
      catch (Exception ex)
      {
        await trx.RollbackAsync();
        //  _logger.LogError(ex, "Bulk insert failed");
        return StatusCode(500, "Database error while inserting users.");
      }

      // 6️⃣ Optional: write passwords to log (remove in prod!)
      var logFilePath = "BulkUserLog.txt";
      await using (var sw = new StreamWriter(logFilePath, append: true))
      {
        foreach (var t in toInsert)
        {
          await sw.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {t.user.ADID} | {t.PlainPassword}");
        }
      }

      // Optionally send e‑mails here (loop & await _emailService.SendEmailAsync)
      // disabled until SMTP available.

      // 7️⃣ Build response
      var insertedSummary = toInsert.Select(t => new
      {
        t.user.ADID,
        t.user.Name,
        t.user.Designation,
        t.user.BadgeId,
        t.user.RoleId,
        t.user.LeadADID,
        t.user.ManagerADID,
        PlainPassword = t.PlainPassword   // ⚠️ remove for prod if unsafe
      });

      return Ok(new
      {
        inserted = insertedSummary,
        skipped
      });
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

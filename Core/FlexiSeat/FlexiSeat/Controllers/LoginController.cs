using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.AuthenticationDTOs;
using FlexiSeat.Helpers;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly IEmailService _emailService;
        private readonly SymmetricSecurityKey _key;
        public LoginController(FlexiSeatDbContext context, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]!));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _context.UserLogins.FindAsync(dto.ADID);
            if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid ADID/Password");

            var userData = await _context.Users.FirstOrDefaultAsync(f => f.ADID == dto.ADID);
            var token = await CreateToken(userData!);
            return Ok(new { token = token });
        }

        private async Task<string> CreateToken(User appUser)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(f => f.Id == appUser.RoleId);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, appUser.ADID.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, appUser.Name),
                new Claim("Role", role!.Name)                
            };

            if(!string.IsNullOrWhiteSpace(Convert.ToString(appUser!.Manager)))
            {
                //var managerClaim = new Claim("Manager ADID", appUser!.Manager);
                claims.Add(new Claim("Manager ADID", Convert.ToString(appUser!.ManagerADID)));
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Normalize ADID
            string normalizedAdid = dto.ADID.Trim().ToUpper();

            // Find user login record
            var userLogin = await _context.UserLogins.FindAsync(normalizedAdid);
            if (userLogin == null)
                return Unauthorized("Invalid ADID/Password");

            // Verify old password
            var isOldPwdMatching = PasswordHelper.VerifyPassword(dto.OldPassword, userLogin.PasswordHash);

            if (!isOldPwdMatching)
            {
                return Unauthorized(new { message = "Old password is incorrect." });
            }

            ///*Need to remove - writing pwd to file logic - only for test purpose*/
            //var logFilePath = "Log.txt";

            //using (var writer = new StreamWriter(logFilePath, append: true))
            //{
            //    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | ADID: {normalizedAdid} | Password: {dto.NewPassword}");
            //}

            //Hash new password & update
            userLogin.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Normalize ADID
            string normalizedAdid = dto.ADID.Trim().ToUpper();

            // Find user login record
            var userLogin = await _context.UserLogins.FindAsync(normalizedAdid);
            if (userLogin == null)
                return Unauthorized("Invalid ADID");

            // Generate a new temporary password (for example: 8 random characters)
            string tempPassword = PasswordHelper.Generate();

            ///*Need to remove - writing pwd to file logic - only for test purpose*/
            //var logFilePath = "Log.txt";

            //using (var writer = new StreamWriter(logFilePath, append: true))
            //{
            //    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | ADID: {normalizedAdid} | Password: {tempPassword}");
            //}

            // Hash the temporary password
            userLogin.PasswordHash = PasswordHelper.HashPassword(tempPassword);

            // Save changes
            await _context.SaveChangesAsync();

              var toEmail = userLogin.ADID + "@ups.com";

            await _emailService.SendEmailAsync(toEmail,"", "UPS Flexiseat - Temporary Password", $"Your new temporary password is: {tempPassword}");


            return Ok(new
            {
                message = "Temporary password has been set. Please check your email."
            });
        }

    }
}

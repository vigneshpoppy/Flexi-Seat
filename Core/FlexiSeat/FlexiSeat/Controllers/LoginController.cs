using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO;
using FlexiSeat.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace FlexiSeat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly ILogger<FlexiSeatController> _logger;
        public LoginController(ILogger<FlexiSeatController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _context.EmployeeLogins.AnyAsync(e => e.ADID == dto.ADID))
                return Conflict("ADID already exists.");

            var newUser = new EmployeeLogin
            {
                ADID = dto.ADID,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Role = dto.Role,
                OTP=dto.OTP
            };

            _context.EmployeeLogins.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }
        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.EmployeeLogins.FindAsync(dto.ADID);
            if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid ADID or password.");

            // Generate OTP
            string otp = new Random().Next(100000, 999999).ToString();
            user.OTP = otp;
            user.OTPGeneratedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Login successful. Use OTP to proceed.", otp = otp });
        }

        // POST: api/Login/ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto dto)
        {
            var user = await _context.EmployeeLogins.FindAsync(dto.ADID);
            if (user == null || !PasswordHelper.VerifyPassword(dto.OldPassword, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }

        // POST: api/Login/VerifyOtp
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpDto dto)
        {
            var user = await _context.EmployeeLogins.FindAsync(dto.ADID);
            if (user == null || user.OTP != dto.OTP)
                return Unauthorized("Invalid OTP.");

            var expiry = user.OTPGeneratedAt?.AddMinutes(5);
            if (expiry < DateTime.UtcNow)
                return Unauthorized("OTP expired.");

            return Ok(new { message = "OTP verified." });
        }
    }

}

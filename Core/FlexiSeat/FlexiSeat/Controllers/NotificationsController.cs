using FlexiSeat.DbContext;
using FlexiSeat.DTO.DashboardDTOs;
using FlexiSeat.DTO.EmailDTOs;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly IEmailService _emailService;
        public NotificationsController(FlexiSeatDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("AdminRequest")]
        public async Task<IActionResult> SendAdminRequestEmail([FromBody] TicketEmailDTO dto)
        {
            // Fetch all admin emails
            var adminEmails = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Name.ToUpper() == "ADMIN")
                .Select(u => u.ADID+"@ups.com")
                .ToListAsync();

            if (!adminEmails.Any())
            {
                return BadRequest("No admin users found.");
            }

            // Join emails as comma-separated string
            var toEmails = string.Join(",", adminEmails);
            var userEmailID = dto.ADID + "@ups.com";

            var subject = $"UPS Flexiseat - Ticket {dto.Id}";
            var body = $@"
        <html>
          <body>
            <p>Requestor ADID: <b>{dto.ADID}</b></p>
            <p>{dto.Body}</p>
            <p>Thanks</p>
          </body>
        </html>";

            await _emailService.SendEmailAsync(toEmails, userEmailID, subject, body);

            return Ok();
        }

    }
}

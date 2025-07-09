using Microsoft.AspNetCore.Mvc;

namespace FlexiSeat.Controllers
{
    using global::FlexiSeat.Data;
    using global::FlexiSeat.DbContext;
    using global::FlexiSeat.DTO;
    using Microsoft.AspNetCore.Mvc;
    using System;


    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly ILogger<FlexiSeatController> _logger;
        public AdminController(ILogger<FlexiSeatController> logger, FlexiSeatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // POST: api/Admin/AddJoinee
        [HttpPost("AddJoinee")]
        public async Task<IActionResult> AddJoinee([FromBody] NewJoineeDto dto)
        {
            // Check if employee already exists
            if (await _context.Employees.FindAsync(dto.ADID) != null)
            {
                return Conflict($"Employee with ADID '{dto.ADID}' already exists.");
            }

            // Check if BadgeID already exists
            if (_context.Employees.Any(e => e.BadgeID == dto.BadgeID))
            {
                return Conflict($"BadgeID '{dto.BadgeID}' is already in use.");
            }

            // 🔍 Lookup Manager Name
            var manager = await _context.Employees.FindAsync(dto.ManagerADID);
            string managerName = manager != null ? $"{manager.FirstName} {manager.LastName}" : "Unknown";

            // Create new employee
            var employee = new Employee
            {
                UserADID = dto.ADID,
                ManagerADID = dto.ManagerADID,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BadgeID = dto.BadgeID,
                ManagerName = managerName,
                UserName = dto.UserName,
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Joinee added successfully", employee });
        }

        // PUT: api/Admin/UpdateJoinee/{adid}
        [HttpPut("UpdateJoinee/{adid}")]
        public async Task<IActionResult> UpdateJoinee(string adid, [FromBody] NewJoineeDto dto)
        {
            // Find existing employee by ADID
            var employee = await _context.Employees.FindAsync(adid);
            if (employee == null)
            {
                return NotFound($"Employee with ADID '{adid}' not found.");
            }

            // Check if BadgeID is being updated to one that already exists on a different employee
            if (_context.Employees.Any(e => e.BadgeID == dto.BadgeID && e.UserADID != adid))
            {
                return Conflict($"BadgeID '{dto.BadgeID}' is already in use by another employee.");
            }

            // Lookup Manager Name
            var manager = await _context.Employees.FindAsync(dto.ManagerADID);
            string managerName = manager != null ? $"{manager.FirstName} {manager.LastName}" : "Unknown";

            // Update employee details
            employee.ManagerADID = dto.ManagerADID;
            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.BadgeID = dto.BadgeID;
            employee.ManagerName = managerName;
            employee.UserName = dto.UserName;

            // Save changes
            await _context.SaveChangesAsync();

            return Ok(new { message = "Joinee updated successfully", employee });
        }

        // DELETE: api/Admin/DeleteJoinee/{adid}
        [HttpDelete("DeleteJoinee/{adid}")]
        public async Task<IActionResult> DeleteJoinee(string adid)
        {
            var employee = await _context.Employees.FindAsync(adid);
            if (employee == null)
            {
                return NotFound($"Employee with ADID '{adid}' not found.");
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Joinee deleted successfully" });
        }

        // GET: api/Admin/GetJoinee/{adid}
        [HttpGet("GetJoinee/{adid}")]
        public async Task<IActionResult> GetJoinee(string adid)
        {
            var employee = await _context.Employees.FindAsync(adid);
            if (employee == null)
            {
                return NotFound($"Employee with ADID '{adid}' not found.");
            }

            return Ok(employee);
        }
    }

}

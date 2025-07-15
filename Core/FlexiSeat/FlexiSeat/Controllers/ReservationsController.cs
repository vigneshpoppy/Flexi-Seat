using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.ReservationDTOs;
using FlexiSeat.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Messaging;

namespace FlexiSeat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly FlexiSeatDbContext _context;
        private readonly IEmailService _emailService;
        public ReservationsController(FlexiSeatDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Normalize ADIDs
            string userAdid = dto.UserADID.Trim().ToUpper();
            string reservedByAdid = dto.ReservedByADID.Trim().ToUpper();

            // Check if User exists
            var user = await _context.Users.FindAsync(userAdid);
            if (user == null)
                return NotFound($"User with ADID '{userAdid}' not found.");

            // Check if ReservedBy user exists
            var reservedBy = await _context.Users.FindAsync(reservedByAdid);
            if (reservedBy == null)
                return NotFound($"ReservedBy user with ADID '{reservedByAdid}' not found.");

            var isUserAlreadyBooked = await _context.Reservations
                .AnyAsync(r => r.ReservedDate == dto.ReservedDate && r.UserADID == userAdid);

            if (isUserAlreadyBooked)
            {
                return Conflict("User already reserved for the selected date.");
            }

            // Check if Seat exists
            var seat = await _context.Seats
                       .FirstOrDefaultAsync(s => s.ID == dto.SeatID && s.IsActive);

            if (seat == null)
                return NotFound($"Active seat with ID '{dto.SeatID}' not found.");

            // Optional: Check if this seat is already reserved for this date
            bool alreadyReserved = await _context.Reservations
                .AnyAsync(r => r.SeatID == dto.SeatID && r.ReservedDate == dto.ReservedDate);
            if (alreadyReserved)
                return Conflict("Seat is already reserved for the selected date.");

            // Create reservation entity
            var reservation = new Reservation
            {
                UserADID = userAdid,
                SeatID = dto.SeatID,
                ReservedDate = dto.ReservedDate.Date,
                InsertedOn = DateTime.Now,
                IsNotified = false,
                Status = "Booked",
                ReservedByADID = reservedByAdid
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var toEmail = userAdid + "@ups.com";
            var subject = $"UPS Flexiseat - Seat Booking";
            var body = $@"
            <html>
              <body>
                <p>Booking Date: <b>{dto.ReservedDate.ToString("yyyy-MM-dd")}</b></p>
                <p>Seat Number: <b>{seat.Number}</b></p>
                <p>Status: <b>Booked</b></p>
                <p>Thanks</p>
              </body>
            </html>";

            await _emailService.SendEmailAsync(toEmail, "", subject, body);

            return Ok(new { message = "Reservation created successfully.", reservation.ID });
        }

        [HttpPost("BulkCreate")]
        public async Task<IActionResult> BulkCreateReservations([FromBody] List<CreateReservationDTO> reservations)
        {
            if (reservations == null || !reservations.Any())
                return BadRequest("No reservation data provided.");

            var emailMsg = new List<string>();
            var now = DateTime.UtcNow;
            var reserved = new List<Reservation>();
            var skipped = new List<object>();

            foreach (var dto in reservations)
            {
                var exists = await _context.Reservations.AnyAsync(r =>
                    r.SeatID == dto.SeatID &&
                    r.ReservedDate == dto.ReservedDate
                );

                var seat = await _context.Seats
                       .FirstOrDefaultAsync(s => s.ID == dto.SeatID && s.IsActive);

                if (exists)
                {
                    skipped.Add(new
                    {
                        dto.SeatID,
                        dto.ReservedDate,
                        Reason = "Seat already reserved for the selected date and time."
                    });
                    emailMsg.Add($"Seat Number - {seat.Number} already booked for {dto.ReservedDate.ToString("yyyy-MM-dd")} by another user. Unable to book for user {dto.UserADID}");
                    continue;
                }

                var isUserAlreadyBooked = await _context.Reservations
                .AnyAsync(r => r.ReservedDate == dto.ReservedDate && r.UserADID == dto.UserADID);

                if (isUserAlreadyBooked)
                {
                    skipped.Add(new
                    {
                        dto.SeatID,
                        dto.ReservedDate,
                        Reason = $"User {dto.UserADID} already booked another seat for the selected date"
                    });
                    emailMsg.Add($"User {dto.UserADID} already booked another seat for {dto.ReservedDate.ToString("yyyy-MM-dd")}. Unable to book seat number {seat.Number}");
                    continue;
                }
                var reservation = new Reservation
                {
                    UserADID = dto.UserADID,
                    SeatID = dto.SeatID,
                    ReservedDate = dto.ReservedDate,
                    InsertedOn = now,
                    UpdatedOn = null,
                    IsNotified = false,
                    ReservedByADID = dto.ReservedByADID,
                    Status = "Booked"
                };

                reserved.Add(reservation);
                _context.Reservations.Add(reservation);
                emailMsg.Add($"Seat Number {seat.Number} booked for user {dto.UserADID}");
            }

            await _context.SaveChangesAsync();

            //Send email
            var reservedByADID = reservations.FirstOrDefault().ReservedByADID;
            var toEmail = reservedByADID + "@ups.com";
            var subject = $"UPS Flexiseat - Seat Booking";
            var body = string.Empty;
            foreach (var msg in emailMsg)
            {
                body = body + $"<p>{msg}</p>";
            }
            await _emailService.SendEmailAsync(toEmail, "", subject, body);

            return Ok(new
            {
                Reserved = reserved.Select(r => new
                {
                    r.ID,
                    r.UserADID,
                    r.SeatID,
                    r.ReservedDate,
                    r.CheckInTime,
                    r.CheckOutTime,
                    r.ReservedByADID
                }),
                Skipped = skipped
            });
        }

        [HttpGet("AvailableZones")]
        public async Task<IActionResult> GetAvailableZones([FromQuery] string adid, [FromQuery] DateTime date)
        {
            if (string.IsNullOrWhiteSpace(adid))
                return BadRequest("ADID is required.");

            string normalizedAdid = adid.Trim().ToUpper();

            // Get the user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ADID == normalizedAdid);
            if (user == null)
                return NotFound("User not found.");

            string? managerAdid = user.ManagerADID;

            // Get all seat IDs reserved for the given date
            var reservedSeatIds = await _context.Reservations
                .Where(r => r.ReservedDate.Date == date.Date)
                .Select(r => r.SeatID)
                .ToListAsync();

            // Get all active and unreserved seats
            var availableSeats = await _context.Seats
                .Where(s => s.IsActive && !reservedSeatIds.Contains(s.ID))
                .Include(s => s.Zone)
                .ToListAsync();

            if (!availableSeats.Any() && availableSeats != null)
                return NoContent(); // No available seats

            // Group seats by ZoneId
            var seatGroups = availableSeats
                .GroupBy(s => s.ZoneId)
                .ToDictionary(g => g.Key, g => g.Count());

            List<Zone> zones;
            Dictionary<int, int> seatCounts = seatGroups;

            if (!string.IsNullOrEmpty(managerAdid))
            {
                // Get zones mapped to manager via OrgSeatPool
                var managerZoneIds = await _context.OrgSeatPools
                    .Where(p => p.ManagerADID == managerAdid)
                    .Select(p => p.ZoneId)
                    .ToListAsync();

                // Filter zones that are active and have available seats
                zones = await _context.Zones
                    .Where(z => z.IsActive && managerZoneIds.Contains(z.ID) && seatCounts.Keys.Contains(z.ID))
                    .ToListAsync();

                // If manager zones have no available seats or all inactive, fallback to all active zones with seats
                if (!zones.Any())
                {
                    zones = await _context.Zones
                        .Where(z => z.IsActive && seatCounts.Keys.Contains(z.ID))
                        .ToListAsync();
                }
            }
            else
            {
                // No manager, get all active zones with available seats
                zones = await _context.Zones
                    .Where(z => z.IsActive && seatCounts.Keys.Contains(z.ID))
                    .ToListAsync();
            }

            var result = zones.Select(z => new GetReservationZoneDTO
            {
                ID = z.ID,
                Name = z.Name,
                Description = z.Description,
                IsActive = z.IsActive,
                ManagerADID = _context.OrgSeatPools.FirstOrDefault(p => p.ZoneId == z.ID)?.ManagerADID,
                ManagerName = _context.OrgSeatPools
                    .Include(p => p.Manager)
                    .FirstOrDefault(p => p.ZoneId == z.ID)?.Manager?.Name,
                AvailableSeatCount = seatCounts.ContainsKey(z.ID) ? seatCounts[z.ID] : 0
            }).ToList();

            return Ok(result);
        }

        [HttpGet("zone/{zoneIds}/seats")]
        public async Task<IActionResult> GetSeatsByZoneAndDate(string zoneIds, [FromQuery] DateTime date)
        {
            List<GetReservationSeatDTO> lstReservations = new List<GetReservationSeatDTO>();
            var zoneIdList = zoneIds.Split(',');
            foreach (var zId in zoneIdList)
            {
                var zoneId = Convert.ToInt32(zId);
                // Validate zone
                var zone = await _context.Zones.FirstOrDefaultAsync(z => z.ID == zoneId);
                if (zone == null)
                    return NotFound($"Zone with ID {zoneId} not found.");

                // Get manager info from OrgSeatPool
                var orgSeatPool = await _context.OrgSeatPools
                    .Include(p => p.Manager)
                    .FirstOrDefaultAsync(p => p.ZoneId == zoneId);

                string? managerAdid = orgSeatPool?.ManagerADID;
                string? managerName = orgSeatPool?.Manager?.Name;

                // Get seat IDs booked on given date in this zone
                var bookedSeatIds = await _context.Reservations
                    .Where(r => r.ReservedDate.Date == date.Date && r.Seat.ZoneId == zoneId)
                    .Select(r => r.SeatID)
                    .ToListAsync();

                var reservations = await _context.Reservations
                    .Where(r => r.ReservedDate.Date == date.Date && r.Seat.ZoneId == zoneId)
                    .ToListAsync();

                // Get all seats in the zone
                var seats = await _context.Seats
                  .Where(s => s.ZoneId == zoneId)
                  .ToListAsync();

                var result = seats.Select(s => new GetReservationSeatDTO
                {
                    ID = s.ID,
                    Number = s.Number,
                    ZoneId = s.ZoneId,
                    ZoneName = zone.Name,
                    ManagerADID = managerAdid,
                    ManagerName = managerName,
                    Status = bookedSeatIds.Contains(s.ID) ? reservations.Where(r => r.SeatID == s.ID).Select(r => r.Status).FirstOrDefault() : "Available",
                    IsActive = s.IsActive
                });
                lstReservations.AddRange(result);
            }
            return Ok(lstReservations);
        }
    }
}

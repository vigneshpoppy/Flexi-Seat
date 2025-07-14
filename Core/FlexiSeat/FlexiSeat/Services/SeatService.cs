using FlexiSeat.Data;
using FlexiSeat.DbContext;
using FlexiSeat.DTO.WhatsappDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Services
{
  public class SeatService
  {
    private readonly FlexiSeatDbContext _context;
    public SeatService(FlexiSeatDbContext context)
    {
      _context = context;
    }

    public async Task<WhatsAppReserveResponseDto> ReserveAsync(
       string phone, int seatId, DateTime date)
    {
      // 1) Map phone → employee
      var emp = await _context.Employees
                         .SingleOrDefaultAsync(e => e.PhoneNumber == phone);
      if (emp is null)
        return new WhatsAppReserveResponseDto
        {
          IsSuccess = false,
          Message = $"Your phone number is not registered."
        };

      // 2) Reject past dates
      if (DateOnly.FromDateTime(date) < DateOnly.FromDateTime(DateTime.Today))
        return new WhatsAppReserveResponseDto
        {
          IsSuccess = false,
          Message = $"Date is in the past."
        };      

      // 3) Check seat clash
      bool clash = await _context.Reservations.AnyAsync(r =>
          r.SeatID == seatId && r.ReservedDate ==Convert.ToDateTime(date));
      if (clash)
        return new WhatsAppReserveResponseDto
        {
          IsSuccess = false,
          Message = $"Seat {seatId} is already booked on {date:dd MMM yyyy}."
        };

      // 4) Create reservation
      var res = new Reservation
      {
        SeatID = seatId,
        ReservedDate = date,
        ReservedByADID = emp.EmployeeADID,
        InsertedOn = DateTime.UtcNow,
        UserADID=emp.EmployeeADID
      };
      _context.Reservations.Add(res);
      await _context.SaveChangesAsync();
      return new WhatsAppReserveResponseDto
      {
        IsSuccess = true,
        Message = $"Seat {seatId} reserved for {date:dd MMM yyyy}."
      };
    }
  }

}

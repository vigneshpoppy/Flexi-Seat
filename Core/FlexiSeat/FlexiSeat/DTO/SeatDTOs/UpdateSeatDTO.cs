using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.SeatDTOs
{
  public class UpdateSeatDTO
  {
      [StringLength(20, ErrorMessage = "Seat number cannot exceed 20 characters.")]
      public string? Number { get; set; }

      public int ZoneId { get; set; }
      public bool? IsActive { get; set; }
  }
}

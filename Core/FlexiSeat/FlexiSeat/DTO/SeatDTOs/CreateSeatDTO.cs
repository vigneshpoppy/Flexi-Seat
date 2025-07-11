using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.SeatDTOs
{
    public class CreateSeatDTO
    {
        [Required(ErrorMessage = "Seat number is required.")]
        [StringLength(20, ErrorMessage = "Seat number cannot exceed 20 characters.")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Zone ID is required.")]
        public int ZoneId { get; set; }
    }
}

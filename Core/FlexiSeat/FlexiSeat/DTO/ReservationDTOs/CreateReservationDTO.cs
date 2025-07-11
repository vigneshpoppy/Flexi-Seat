using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.ReservationDTOs
{
    public class CreateReservationDTO
    {
        [Required(ErrorMessage = "User ADID is required.")]
        [StringLength(20, ErrorMessage = "User ADID cannot exceed 20 characters.")]
        public string UserADID { get; set; }

        [Required(ErrorMessage = "Seat ID is required.")]
        public int SeatID { get; set; }

        [Required(ErrorMessage = "Reserved date is required.")]
        [DataType(DataType.Date)]
        public DateTime ReservedDate { get; set; }

        [Required(ErrorMessage = "ReservedBy ADID is required.")]
        [StringLength(20, ErrorMessage = "ReservedBy ADID cannot exceed 20 characters.")]
        public string ReservedByADID { get; set; }
    }
}

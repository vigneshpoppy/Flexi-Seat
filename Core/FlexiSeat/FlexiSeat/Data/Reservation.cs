using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlexiSeat.Data
{
    public class Reservation
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string UserADID { get; set; }

        [ForeignKey("UserADID")]
        public User User { get; set; }

        public string? ReservedByADID { get; set; }

        [ForeignKey("ReservedByADID")]
        public User? ReservedBy { get; set; }

        [Required]
        public int SeatID { get; set; }

        [ForeignKey("SeatID")]
        public Seat Seat { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime ReservedDate { get; set; }

        [MaxLength(10)]
        public string? CheckInTime { get; set; }

        [MaxLength(10)]
        public string? CheckOutTime { get; set; }

        [Required]
        public DateTime InsertedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool IsSmsSent { get; set; }
    }
}

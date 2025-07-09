using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlexiSeat.Data
{
    public class Seat
    {
        [Key]
        public int ID { get; set; }

        // Foreign key to Zone entity
        public int ZoneId { get; set; }

        [ForeignKey("ZoneId")]
        public Zone Zone { get; set; }

        public bool IsActive { get; set; }
    }
}

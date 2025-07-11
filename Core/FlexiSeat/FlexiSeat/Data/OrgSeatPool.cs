using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class OrgSeatPool
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ZoneId { get; set; }

        [ForeignKey("ZoneId")]
        public Zone Zone { get; set; }

        public string? ManagerADID { get; set; }

        [ForeignKey("ManagerADID")]
        public User? Manager { get; set; }

        [Required]
        public int SeatsAllotted { get; set; }
    }
}

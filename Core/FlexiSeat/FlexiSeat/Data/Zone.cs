using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class Zone
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        [MaxLength(150)]
        public string LocationName { get; set; }

        public bool IsActive { get; set; }

        public string? ManagerADID { get; set; }

        [ForeignKey("ManagerADID")]
        public User? Manager { get; set; }
    }
}

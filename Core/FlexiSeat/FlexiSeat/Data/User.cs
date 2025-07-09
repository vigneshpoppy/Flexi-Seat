using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class User
    {
        [Key]
        [Required]
        public string ADID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Designation { get; set; }

        [MaxLength(50)]
        public string BadgeID { get; set; }

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public AppRole Role { get; set; }
        
        public string? LeadADID { get; set; }

        [ForeignKey("LeadADID")]
        public User? Lead { get; set; }

        public string? ManagerADID { get; set; }
        [ForeignKey("ManagerADID")]
        public User? Manager { get; set; }
    }
}

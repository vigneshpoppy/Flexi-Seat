using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class Employee
    {
        [Key]
        [StringLength(100)]
        public string UserADID { get; set; }

        [StringLength(100)]
        public string ManagerADID { get; set; }

        [Required]
        public string UserName { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string ManagerName { get; set; } = string.Empty;

        [Required]
        [StringLength(11)]
        public string BadgeID { get; set; }
    }
}

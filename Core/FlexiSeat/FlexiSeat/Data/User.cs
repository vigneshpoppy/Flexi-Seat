using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class User
    {
        private string _adid;
        private string _name;
        private string _designation;
        private string _badgeId;

        [Key]
        [StringLength(20)]
        public string ADID
        {
            get => _adid;
            set => _adid = value?.Trim().ToUpper();
        }

        [Required]
        [StringLength(100)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim().ToUpper();
        }

        [StringLength(120)]
        public string? Designation
        {
            get => _designation;
            set => _designation = value?.Trim().ToUpper();
        }

        [Required]
        [StringLength(50)]
        public string BadgeId
        {
            get => _badgeId;
            set => _badgeId = value?.Trim().ToUpper();
        }

        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public string? LeadADID { get; set; }

        [ForeignKey("LeadADID")]
        public User? Lead { get; set; }

        public string? ManagerADID { get; set; }

        [ForeignKey("ManagerADID")]
        public User? Manager { get; set; }
    }
}

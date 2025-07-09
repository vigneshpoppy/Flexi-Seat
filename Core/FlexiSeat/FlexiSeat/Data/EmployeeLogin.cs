using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class EmployeeLogin
    {
        [Key]
        public string ADID { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string OTP { get; set; }
        public DateTime? OTPGeneratedAt { get; set; }

        // Foreign key property
        public int AppRoleID { get; set; }

        // Navigation property
        public AppRole AppRole { get; set; }
    }

}

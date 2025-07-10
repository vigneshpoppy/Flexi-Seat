using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class UserLogin
    {
        [Key]
        public string ADID { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPGeneratedAt { get; set; }
    }

}

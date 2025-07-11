using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class UserLogin
    {
        [Key]
        [StringLength(20)]
        public string ADID { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}

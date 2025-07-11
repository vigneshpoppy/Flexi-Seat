using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.AuthenticationDTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "ADID is required.")]
        [StringLength(20, ErrorMessage = "ADID cannot exceed 20 characters.")]
        public string ADID { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}

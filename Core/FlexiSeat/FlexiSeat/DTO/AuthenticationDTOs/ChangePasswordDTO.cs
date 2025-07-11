using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.AuthenticationDTOs
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "ADID is required.")]
        [StringLength(20, ErrorMessage = "ADID cannot exceed 20 characters.")]
        public string ADID { get; set; }

        [Required(ErrorMessage = "Old Password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Old Password is required.")]
        public string NewPassword { get; set; }
    }
}

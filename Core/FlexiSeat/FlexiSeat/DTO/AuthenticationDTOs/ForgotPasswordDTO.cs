using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.AuthenticationDTOs
{
    public class ForgotPasswordDTO
    {
      [Required(ErrorMessage = "ADID is required.")]
      [StringLength(20, ErrorMessage = "ADID cannot exceed 20 characters.")]
      public string ADID { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
namespace FlexiSeat.DTO.UserDTOs
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "ADID is required.")]
        [StringLength(20, ErrorMessage = "ADID cannot exceed 20 characters.")]
        public string ADID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [StringLength(120, ErrorMessage = "Designation cannot exceed 120 characters.")]
        public string? Designation { get; set; }

        [Required(ErrorMessage = "Badge ID is required.")]
        [StringLength(50, ErrorMessage = "Badge ID cannot exceed 50 characters.")]
        public string BadgeId { get; set; }

        [Required(ErrorMessage = "Role ID is required.")]
        public int RoleId { get; set; }

        public string? LeadADID { get; set; }

        public string? ManagerADID { get; set; }
    }

}

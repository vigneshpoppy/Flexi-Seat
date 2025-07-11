using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.UserDTOs
{
    public class UpdateUserDTO
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [StringLength(120, ErrorMessage = "Designation cannot exceed 120 characters.")]
        public string? Designation { get; set; }

        [StringLength(50, ErrorMessage = "Badge ID cannot exceed 50 characters.")]
        public string? BadgeId { get; set; }

        public int RoleId { get; set; }

        public string? LeadADID { get; set; }

        public string? ManagerADID { get; set; }
  }
}

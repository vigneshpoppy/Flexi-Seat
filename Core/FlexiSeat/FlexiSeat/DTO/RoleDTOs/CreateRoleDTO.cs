using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.RoleDTOs
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string? Description { get; set; }
  }
}

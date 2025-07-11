using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.DTO.ZoneDTOs
{
    public class CreateZoneDTO
    {
        [Required(ErrorMessage = "Zone name is required.")]
        [StringLength(50, ErrorMessage = "Zone name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string? Description { get; set; }
    }
}

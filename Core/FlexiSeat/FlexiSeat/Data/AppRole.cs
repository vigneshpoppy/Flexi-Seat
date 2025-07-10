using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class AppRole
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

    }
}

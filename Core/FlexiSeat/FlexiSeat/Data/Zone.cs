using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class Zone
    {
        private string _name;
        private string _description;
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name
        {
          get => _name;
          set => _name = value?.Trim().ToUpper();
        }

        [StringLength(100)]
        public string? Description
        {
          get => _description;
          set => _description = value?.Trim().ToUpper();
        }

        [Required]
        public bool IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        private string _name;

        [Required]
        [StringLength(50)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim().ToUpper();
        }
    }
}

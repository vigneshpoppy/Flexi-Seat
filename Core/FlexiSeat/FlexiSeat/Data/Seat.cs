using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
      public class Seat
      {
          private string _number;

          [Key]
          public int ID { get; set; }

          [Required]
          [StringLength(20)]
          public string Number
          {
            get => _number;
            set => _number = value?.Trim().ToUpper();
          }

          [Required]
          public int ZoneId { get; set; }

          [ForeignKey("ZoneId")]
          public Zone Zone { get; set; }

          [Required]
          public bool IsActive { get; set; }
      }
}

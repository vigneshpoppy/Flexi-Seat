using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class Designation
    {
        [Key]
        public string ADID { get; set; }

        [Range(1, 5)]
        public int Position { get; set; }
    }
}

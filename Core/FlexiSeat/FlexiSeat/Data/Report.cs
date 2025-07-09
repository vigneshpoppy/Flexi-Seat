using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
    public class Report
    {
        [Key]
        public int DocumentID { get; set; }

        [Required]
        public string ADID { get; set; }

        [Required]
        public string DocumentURL { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
  public class Employee
  {
    [Key]                              // PK
    [StringLength(20)]
    public string EmployeeADID { get; set; } = null!;

    public long EmployeeNo { get; set; }

    [MaxLength(20)]
    public string PhoneNumber { get; set; } = null!;

    public DateTime InsertedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    
  }
}

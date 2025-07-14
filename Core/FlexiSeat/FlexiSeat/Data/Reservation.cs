using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlexiSeat.Data
{
  public class Reservation
  {
    private string _userADID;
    private string _reservedByADID;

    [Key]
    public int ID { get; set; }

    [Required]
    [StringLength(20)]
    public string UserADID
    {
      get => _userADID;
      set => _userADID = value?.Trim().ToUpper();
    }

    [ForeignKey("UserADID")]
    public User User { get; set; }

    [Required]
    public int SeatID { get; set; }

    [ForeignKey("SeatID")]
    public Seat Seat { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime ReservedDate { get; set; }

    [StringLength(5)]
    public string? CheckInTime { get; set; }

    [StringLength(5)]
    public string? CheckOutTime { get; set; }

    [Required]
    public DateTime InsertedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    [Required]
    public bool IsNotified { get; set; }

    [Required]
    [StringLength(20)]
    public string ReservedByADID
    {
      get => _reservedByADID;
      set => _reservedByADID = value?.Trim().ToUpper();
    }

    [ForeignKey("ReservedByADID")]
    public User ReservedBy { get; set; }
    public Employee Employee { get; set; }

  }
}

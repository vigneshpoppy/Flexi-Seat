namespace FlexiSeat.DTO.ReportDTO
{
  public class AttendanceReportDTO
  {
    public DateTime Date { get; set; }
    public string CheckInTime { get; set; }
    public string CheckOutTime { get; set; }
    public double? TotalHours
    {
      get
      {
        if (DateTime.TryParse(CheckInTime, out var checkIn) && DateTime.TryParse(CheckOutTime, out var checkOut))
        {
          return (checkOut - checkIn).TotalHours;
        }
        return null;
      }
    }
    public bool IsLateCheckIn { get; set; }
    public bool IsMissingCheckOut { get; set; }
  }
}

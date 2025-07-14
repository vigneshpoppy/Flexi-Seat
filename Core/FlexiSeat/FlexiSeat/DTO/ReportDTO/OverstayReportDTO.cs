namespace FlexiSeat.DTO.ReportDTO
{
  public class OverstayReportDTO
  {
    public string ADID { get; set; }
    public string Name { get; set; }
    public string ZoneName { get; set; }
    public string SeatNumber { get; set; }
    public DateTime ReservedDate { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public double DurationHours { get; set; }
  }

}

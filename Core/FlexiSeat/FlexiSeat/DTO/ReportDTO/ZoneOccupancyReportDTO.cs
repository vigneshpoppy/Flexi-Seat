namespace FlexiSeat.DTO.ReportDTO
{
  public class ZoneOccupancyReportDTO
  {
    public int ZoneId { get; set; }
    public string ZoneName { get; set; }
    public int TotalSeats { get; set; }
    public int UniqueBookings { get; set; }
    public int TotalReservations { get; set; }
    public double OccupancyRate => TotalSeats > 0 ? (double)UniqueBookings / TotalSeats * 100 : 0;
  }
}

namespace FlexiSeat.DTO.ReportDTO
{
  public class NoShowReportDTO
  {
    public string ADID { get; set; }
    public string Name { get; set; }
    public string ZoneName { get; set; }
    public DateTime ReservedDate { get; set; }
    public int SeatID { get; set; }
    public string SeatNumber { get; set; }
    public string ReservedByADID { get; set; }
  }

}

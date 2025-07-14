namespace FlexiSeat.DTO.ReportDTO
{
  public class LiveSeatStatusDTO
  {
    public int SeatID { get; set; }
    public string SeatNumber { get; set; }
    public string ZoneName { get; set; }
    public bool IsOccupied { get; set; }
    public string OccupiedByADID { get; set; }
    public string OccupiedByName { get; set; }
    public DateTime? CheckInTime { get; set; }
   public int SeatId { get; set; }           
      public string Zone { get; set; }
   public bool IsReserved { get; set; }
}

}

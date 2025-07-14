namespace FlexiSeat.DTO.ReportDTO
{
  public class ManagerUsageReportDTO
  {
    public string ManagerADID { get; set; }
    public string ManagerName { get; set; }
    public string ZoneName { get; set; }
    public int TotalEmployees { get; set; }
    public int CheckedInCount { get; set; }
    public int NoShowCount { get; set; }
    public double AverageHoursWorked { get; set; }
    public int EmployeeCount { get; set; }
    public int TotalReservations { get; set; }
  }

}

namespace FlexiSeat.DTO.ZoneDTOs
{
  public class UpdateZoneDTO
  {
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? LocationName { get; set; }
    public bool IsActive { get; set; }
    public string? ManagerADID { get; set; }
  }
}

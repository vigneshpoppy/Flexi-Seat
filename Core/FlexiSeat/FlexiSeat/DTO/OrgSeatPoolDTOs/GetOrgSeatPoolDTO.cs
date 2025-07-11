namespace FlexiSeat.DTO.OrgSeatPoolDTOs
{
    public class GetOrgSeatPoolDTO
    {
    public int ID { get; set; }
    public int ZoneId { get; set; }
    public string ZoneName { get; set; }
    public string? ManagerADID { get; set; }
    public string? ManagerName { get; set; }
    public int SeatsAllotted { get; set; }
  }
}

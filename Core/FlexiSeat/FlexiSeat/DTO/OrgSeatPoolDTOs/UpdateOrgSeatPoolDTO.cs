namespace FlexiSeat.DTO.OrgSeatPoolDTOs
{
    public class UpdateOrgSeatPoolDTO
    {
      public int ZoneId { get; set; }
      public string? ManagerADID { get; set; }
      public int? SeatsAllotted { get; set; }
      public bool? ShowRemoveManagerOption { get; set; }
    }
}

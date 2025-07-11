namespace FlexiSeat.DTO.SeatDTOs
{
    public class GetSeatDTO
    {
      public int ID { get; set; }
      public string Number { get; set; }
      public int ZoneId { get; set; }
      public string ZoneName { get; set; }
      public bool IsActive { get; set; }
    }
}

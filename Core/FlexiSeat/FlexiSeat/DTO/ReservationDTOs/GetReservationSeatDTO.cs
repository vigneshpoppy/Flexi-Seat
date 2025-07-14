namespace FlexiSeat.DTO.ReservationDTOs
{
    public class GetReservationSeatDTO
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string? ManagerADID { get; set; }
        public string? ManagerName { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
  }
}

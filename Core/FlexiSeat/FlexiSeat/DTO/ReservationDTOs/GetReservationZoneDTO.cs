namespace FlexiSeat.DTO.ReservationDTOs
{
    public class GetReservationZoneDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? ManagerADID { get; set; }
        public string? ManagerName { get; set; }
        public int AvailableSeatCount { get; set; }
    }
}
